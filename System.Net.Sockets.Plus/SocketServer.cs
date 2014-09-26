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
	public class SocketServer<T> : SocketServer<T, BytePacket>
	{
		public SocketServer()
		{
			this.DefaultDecoder = new ByteDecoder<T>();
			this.DefaultEncoder = new ByteEncoder<T>();
		}
	}
	public class SocketServer<T, TPacket>
	{

		#region Fields
		object NetworkLock = new object();
		//private int bufferSize = 4096;
		#endregion

		#region Properties
		public Socket Server { get; set; }

		public List<SocketClient<T, TPacket>> Clients { get; private set; }

		//public int BufferSize { get { return bufferSize; } set { bufferSize = value; } }

		public IPacketDecoder<T, TPacket> DefaultDecoder { get; set; }
		public IPacketEncoder<T, TPacket> DefaultEncoder { get; set; }
		#endregion

		#region Delegate
		public delegate void SocketEvent(object sender, SocketEventArgs<T, TPacket> args);
		public delegate void SocketErrorEvent(object sender, SocketErrorEventArgs<T, TPacket> args);
		public delegate T SocketConnectEvent(object sender, SocketEventArgs<T, TPacket> args);
		#endregion

		#region Event
		public event SocketEvent OnDisconnect;
		public event SocketErrorEvent OnSocketException;
		public event SocketConnectEvent OnConnectRequest;
		public event SocketEvent OnDataReceived;
		#endregion

		#region Constructor
		public SocketServer()
		{
			Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			Clients = new List<SocketClient<T, TPacket>>();

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
			catch { return false; }
		}
		public bool Setup(EndPoint endp)
		{
			try { Throwable_Setup(endp); return true; }
			catch { return false; }
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
			//Server.Shutdown(SocketShutdown.Both);
			SocketClient<T, TPacket> currentClient;
			for (int i = 0; i < Clients.Count; ++i)
			{
				currentClient = Clients[i];
				currentClient.Close();

			}

			//			Server = null;
		}



		public void Send(SocketClient<T, TPacket> client, TPacket data)
		{
			if (!ConnectCheck(client)) return;
			byte[] byteData = client.Encoder.Encode(data, client);
			Send(client, byteData);

		}

		public void Send(SocketClient<T, TPacket> client, byte[] data)
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
				this.SocketErrorCall(ex, new SocketEventArgs<T, TPacket>(client), SocketErrorType.Send);
			}
		}



		#endregion

		#region PrivateMethods
		private void SocketErrorCall(Exception ex, SocketEventArgs<T, TPacket> socket, SocketErrorType type)
		{
			SocketErrorEventArgs<T, TPacket> eArgs = new SocketErrorEventArgs<T, TPacket>(ex, socket, type);
			if (OnSocketException != null)
			{
				OnSocketException(this, eArgs);
			}
			socket.Client.CallSocketException(this, eArgs);

			ConnectCheck(socket.Client);
			if (Clients.Contains(socket.Client))
				Clients.Remove(socket.Client);
		}

		private bool ConnectCheck(SocketClient<T, TPacket> client)
		{


			if (!client.Client.Connected)
			{
				//CallDisconnect(client);

				return false;
			}

			return true;

		}
		private void CallDisconnect(SocketClient<T, TPacket> client)
		{
			SocketEventArgs<T, TPacket> args = new SocketEventArgs<T, TPacket>(client);

			if (OnDisconnect != null)
			{
				OnDisconnect(this, args);
			}
			client.CallDisconnect(this, args);
			client.Close();
		}
		private SocketClient<T, TPacket> SerchClient(int id)
		{
			return Clients.Where((e) => e.ID == id).First();
		}
		#endregion

		#region CallbackMethods
		private void OnAccept(IAsyncResult ar)
		{
			lock (NetworkLock)
			{

				Socket socket = Server.EndAccept(ar);
				SocketClient<T, TPacket> client = new SocketClient<T, TPacket>(this, socket, DefaultDecoder, DefaultEncoder);
				SocketEventArgs<T, TPacket> args = new SocketEventArgs<T, TPacket>(client);

				try
				{
					T t = default(T);
					if (OnConnectRequest != null)
					{
						t = OnConnectRequest(this, args);
					}

					client.State = t;
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
		}

		private void OnReceived(IAsyncResult ar)
		{

			int clientId = (int)ar.AsyncState;

			SocketClient<T, TPacket> client;
			try
			{
				client = SerchClient(clientId);
			}
			catch (InvalidOperationException) { return; }

			SocketEventArgs<T, TPacket> args = new SocketEventArgs<T, TPacket>(client);
			try
			{
				//if (!ConnectCheck(client)) return;
				int len = client.Client.EndReceive(ar);
				//Console.WriteLine(len);
				if (client.Client.Available < 1)
				{
					CallDisconnect(client);
					Clients.Remove(client);
					return;
				}
				lock (NetworkLock)
				{
					/*byte[] buff = client.Buffer;
					client.Buffer = new byte[len];
					Buffer.BlockCopy(buff, 0, client.Buffer, 0, len);
					if (client.Crypter!= null)
					{
						client.Buffer= client.Crypter.Decrypt(client.Buffer);
					}
					*/

					while (client.Client.Available > 0)
					{
						args.Packet =(TPacket)client.Decoder.Decode(this, client, client.NetworkStream);
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
				SocketClient<T, TPacket> client;
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
				catch (SocketException ex)
				{
					this.SocketErrorCall(ex, new SocketEventArgs<T, TPacket>(client), SocketErrorType.Send);
				}
			}
		}
		#endregion
	}
}
