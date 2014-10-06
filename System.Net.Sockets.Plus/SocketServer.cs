using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets.Plus.BytePackets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public class SocketServer : SocketServer<object>
	{
		public SocketServer()
			: base()
		{

		}
	}
	public class SocketServer<TState> : SocketServer<TState, BytePacket>
	{
		public SocketServer()
			: base()
		{
			this.DefaultDecoder = new ByteDecoder<TState>();
			this.DefaultEncoder = new ByteEncoder<TState>();
		}
	}

	public class SocketServer<TState, TPacket> : SocketServer<TState, TPacket, TPacket>
	{


		public new IPacketDecoder<TState, TPacket, TPacket> DefaultDecoder
		{
			get
			{
				return base.DefaultDecoder;
			}
			set
			{
				base.DefaultDecoder = value;
			}
		}
		public new IPacketEncoder<TState, TPacket, TPacket> DefaultEncoder
		{
			get
			{
				return base.DefaultEncoder;
			}
			set
			{
				base.DefaultEncoder = value;
			}
		}
	}
	public class SocketServer<TState, TSendPacket, TReceivePacket> : IDisposable
	{

		#region Fields
		object NetworkLock = new object();
		//private int bufferSize = 4096;
		#endregion

		#region Properties
		public Socket Server { get; set; }

		public List<SocketClient<TState, TSendPacket, TReceivePacket>> Clients { get; private set; }

		//public int BufferSize { get { return bufferSize; } set { bufferSize = value; } }

		public IPacketDecoder<TState, TSendPacket, TReceivePacket> DefaultDecoder { get; set; }
		public IPacketEncoder<TState, TSendPacket, TReceivePacket> DefaultEncoder { get; set; }


		public IActivator<TState, TSendPacket, TReceivePacket> Activator { get; set; }


		public Exception LastError { get; private set; }

		/// <summary>
		/// エラーが発生した時に呼び出し元にthrowするかどうかを取得、設定します。
		/// (Default: true)
		/// </summary>
		public bool IsThrowProtectEnable { get; set; }

		#endregion

		#region Delegate
		public delegate void SocketEvent(object sender, SocketEventArgs<TState, TSendPacket, TReceivePacket> args);
		public delegate void SocketReceiveEventArgs(object sender, SocketReceiveEventArgs<TState, TSendPacket, TReceivePacket> args);
		public delegate void SocketErrorEvent(object sender, SocketErrorEventArgs<TState, TSendPacket, TReceivePacket> args);
		public delegate void SocketConnectEvent(object sender, SocketConnectEventArgs<TState, TSendPacket, TReceivePacket> args);
		public delegate void SocketDisconnectEvent(object sender, SocketDisconnectEventArgs<TState, TSendPacket, TReceivePacket> args);
		#endregion

		#region Event
		public event SocketDisconnectEvent OnDisconnect;
		public event SocketErrorEvent OnSocketException;
		public event SocketConnectEvent OnConnectRequest;
		public event SocketReceiveEventArgs OnDataReceived;
		#endregion

		#region Constructor
		public SocketServer()
		{
			Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Clients = new List<SocketClient<TState, TSendPacket, TReceivePacket>>();
			Activator = new SimpleActivator<TState, TSendPacket, TReceivePacket>();

			IsThrowProtectEnable = true;
		}

		public SocketServer(IPacketDecoder<TState, TSendPacket, TReceivePacket> decoder, IPacketEncoder<TState, TSendPacket, TReceivePacket> encoder)
			: this()
		{
			this.DefaultDecoder = decoder;
			this.DefaultEncoder = encoder;
		}
		#endregion

		#region PublicMethods
		public bool Setup(int port)
		{
			return Setup("0.0.0.0", port);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool Setup(string ip, int port)
		{
			try
			{
				return Setup(new IPEndPoint(IPAddress.Parse(ip), port));
			}
			catch (Exception e) { LastError = e; return false; }
		}
		public bool Setup(EndPoint endp)
		{
			try { Throwable_Setup(endp); return true; }
			catch (Exception e) { LastError = e; return false; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="endp"></param>
		/// <exception cref="ArgumentNullException">EndPointがNullの時にThrowされます。</exception>
		/// <exception cref="SocketException">ソケットへのアクセスを試みているときにエラーが発生しました。</exception>
		/// <exception cref="ObjectDisposedException">Socketは閉じられています。</exception>
		/// <exception cref="SecurityException">コールスタックの上位にある呼び出し下が、要求された操作おアクセス許可を保持していません。</exception>
		public void Throwable_Setup(EndPoint endp)
		{
			Server.Bind(endp);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="InvalidOperationException">Decoder,Encoderが未設定時</exception>
		public void Start()
		{

			if (DefaultEncoder == null)
				throw new InvalidOperationException("Encoderが未設定です。");
			if (DefaultDecoder == null)
				throw new InvalidOperationException("Decoderが未設定です。");


			Server.Listen(100);
			Server.BeginAccept(OnAccept, null);
		}
		public void Stop()
		{
			SocketClient<TState, TSendPacket, TReceivePacket> currentClient;
			if (Clients.Count > 1)
			{
				for (int i = 0; i < Clients.Count; ++i)
				{
					currentClient = Clients[i];
					currentClient.Close();
				}
			}
			Server.Dispose();

		}



		public void Send(SocketClient<TState, TSendPacket, TReceivePacket> client, TSendPacket data)
		{
			if (!ConnectCheck(client)) return;
			byte[] byteData = client.Encoder.Encode(data, client);
			Send(client, byteData);

		}

		public void Send(SocketClient<TState, TSendPacket, TReceivePacket> client, byte[] data)
		{
			try
			{
				if (!ConnectCheck(client)) return;

				if (client.Crypter != null)
				{
					data = client.Crypter.Encrypt(data, 0, data.Length);
				}

				client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, OnSend, client.ID);
			}
			catch (SocketException ex)
			{
				this.SocketErrorCall(ex, new SocketEventArgs<TState, TSendPacket, TReceivePacket>(client), SocketErrorType.Send);
			}
		}



		#endregion

		#region PrivateMethods
		private void SocketErrorCall(Exception ex, SocketEventArgs<TState, TSendPacket, TReceivePacket> socket, SocketErrorType type)
		{
			LastError = ex;
			SocketErrorEventArgs
			<TState, TSendPacket, TReceivePacket> eArgs = new SocketErrorEventArgs<TState, TSendPacket, TReceivePacket>(ex, socket, type);
			if (OnSocketException != null)
			{
				OnSocketException(this, eArgs);
			}
			socket.Client.CallSocketException(this, eArgs);

			ConnectCheck(socket.Client);
			if (Clients.Contains(socket.Client))
				Clients.Remove(socket.Client);

			if (IsThrowProtectEnable == false)
				throw ex;

		}

		private bool ConnectCheck(SocketClient<TState, TSendPacket, TReceivePacket> client)
		{


			if (!client.Client.Connected)
			{
				//CallDisconnect(client);

				return false;
			}

			return true;

		}
		private void CallDisconnect(SocketClient<TState, TSendPacket, TReceivePacket> client)
		{
			SocketDisconnectEventArgs<TState, TSendPacket, TReceivePacket> args = new SocketDisconnectEventArgs<TState, TSendPacket, TReceivePacket>(client);

			if (OnDisconnect != null)
			{
				OnDisconnect(this, args);
			}
			client.CallDisconnect(this, args);
			client.Close();
		}
		private SocketClient<TState, TSendPacket, TReceivePacket> SerchClient(int id)
		{
			return Clients.Where((e) => e.ID == id).First();
		}
		#endregion

		#region CallbackMethods
		private void OnAccept(IAsyncResult ar)
		{
			lock (NetworkLock)
			{

				try
				{
					Socket socket = Server.EndAccept(ar);
					var client = new SocketClient<TState, TSendPacket, TReceivePacket>(this, socket, DefaultDecoder, DefaultEncoder);
					SocketConnectEventArgs<TState, TSendPacket, TReceivePacket> args = new SocketConnectEventArgs<TState, TSendPacket, TReceivePacket>(client);

					try
					{

						client.State = Activator.Activate(args);


						if (OnConnectRequest != null)
						{
							OnConnectRequest(this, args);
						}

						client.CallConnected(this, args);

						client.ID = Clients.Count;

						Clients.Add(client);

						byte[] buffer = new byte[0];
						client.Client.BeginReceive(buffer, 0, 0, SocketFlags.None, OnReceived, client.ID);
					}
					catch (SocketException ex)
					{
						this.SocketErrorCall(ex, args, SocketErrorType.Connect);
					}
					Server.BeginAccept(OnAccept, null);
				}
				catch (ObjectDisposedException) { }
			}
		}

		private void OnReceived(IAsyncResult ar)
		{

			int clientId = (int)ar.AsyncState;

			SocketClient<TState, TSendPacket, TReceivePacket> client;
			try
			{
				client = SerchClient(clientId);
			}
			catch (InvalidOperationException) { return; }

			SocketReceiveEventArgs<TState, TSendPacket, TReceivePacket> args = new SocketReceiveEventArgs<TState, TSendPacket, TReceivePacket>(client);
			try
			{
				int len = client.Client.EndReceive(ar);
				if (client.Client.Available < 1)
				{
					CallDisconnect(client);
					Clients.Remove(client);
					return;
				}
				lock (NetworkLock)
				{

					while (client.Client.Available > 0)
					{
						args.Packet =(TReceivePacket)client.Decoder.Decode(this, client, client.NetworkStream);

						if (client.Connected == false)
						{
							//切断処理
							client.Close();
							return;
						}
						client.UsingBuffer = client.NetworkStream.UsingBuffer.ToArray();

						if (OnDataReceived != null)
						{
							OnDataReceived(this, args);
						}


						client.CallDataReceived(this, args);
						client.NetworkStream.Flush();
					}

				}
				if (!client.IsClosed)
				{
					byte[] buffer = new byte[0];
					client.Client.BeginReceive(buffer, 0, 0, SocketFlags.None, OnReceived, clientId);
				}


			}
			catch (ArgumentException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Receive);
			}
			catch (SocketPlusException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Receive);
			}
			catch (SocketException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Receive);
			}
			catch (InvalidOperationException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Receive);
			}
		}

		private void OnSend(IAsyncResult ar)
		{
			lock (NetworkLock)
			{
				int clientId = (int)ar.AsyncState;
				SocketClient<TState, TSendPacket, TReceivePacket> client;
				try
				{
					client = SerchClient(clientId);
				}
				catch (InvalidOperationException) { return; }
				if (!ConnectCheck(client)) return;
				try
				{
					client.Client.EndSend(ar);
				}
				catch (ArgumentException ex)
				{

					this.SocketErrorCall(ex, new SocketEventArgs<TState, TSendPacket, TReceivePacket>(client), SocketErrorType.Send);
				}
				catch (SocketException ex)
				{
					this.SocketErrorCall(ex, new SocketEventArgs<TState, TSendPacket, TReceivePacket>(client), SocketErrorType.Send);
				}
			}
		}
		#endregion

		#region IDisposable メンバー

		public void Dispose()
		{
			this.Stop();
		}

		#endregion

		#region PublicStaticMethods

		public static bool CheckPort(int port)
		{
			SocketServer server = new SocketServer();
			bool result = false;
			try
			{
				result = server.Setup(port);
			}
			finally
			{
				server.Stop();
				server = null;
			}

			return result;
		}
		#endregion


	}
}
