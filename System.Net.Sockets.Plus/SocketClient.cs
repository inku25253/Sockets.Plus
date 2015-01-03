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

	public class SocketClient :SocketClient<object>
	{
		public SocketClient()
			: base()
		{

		}
	}
	public class SocketClient<T> :SocketClient<T, BytePacket>
	{
		public SocketClient()
		{
			this.Decoder = new ByteDecoder<T>();
			this.Encoder = new ByteEncoder<T>();

		}
	}
	public class SocketClient<T, TP> :SocketClient<T, TP, TP>
	{
		public new IPacketDecoder<T, TP, TP> Decoder
		{
			get
			{
				return base.Decoder;
			}
			set
			{
				base.Decoder = value;
			}
		}
		public new IPacketEncoder<T, TP, TP> Encoder
		{
			get
			{
				return base.Encoder;
			}
			set
			{
				base.Encoder = value;
			}
		}
	}

	public class SocketClient<T, TSendPacket, TReceivePacket>
	{
		#region Fields
		private ManualResetEvent clientDone = new ManualResetEvent(false);
		#endregion
		#region Properties
		//private int bufferSize = 8192;
		//public int BufferSize { get { return bufferSize; } set { bufferSize = value; } }
		public byte[] UsingBuffer { get; set; }

		private readonly object NetworkLock = new object();

		public Socket Client { get; private set; }

		public SocketServer<T, TSendPacket, TReceivePacket> Server { get; private set; }
		public IActivator<T, TSendPacket, TReceivePacket> Activator { get; set; }

		public T State { get; set; }

		public int ID { get; internal set; }


		public bool Connected { get { return Client.Connected; } }

		public SocketStream<T, TSendPacket, TReceivePacket> NetworkStream { get; set; }

		public IPacketDecoder<T, TSendPacket, TReceivePacket> Decoder { get; set; }
		public IPacketEncoder<T, TSendPacket, TReceivePacket> Encoder { get; set; }

		public IPacketCrypter Crypter { get; set; }

		public bool IsClosed { get; private set; }

		/// <summary>
		/// エラーが発生した時に呼び出し元にthrowするかどうかを取得、設定します。
		/// (Default: true)
		/// </summary>
		public bool IsThrowProtectEnable { get; set; }
		#endregion

		#region Events
		public event SocketServer<T, TSendPacket, TReceivePacket>.SocketDisconnectEvent OnDisconnect;
		public event SocketServer<T, TSendPacket, TReceivePacket>.SocketErrorEvent OnSocketException;
		public event SocketServer<T, TSendPacket, TReceivePacket>.SocketConnectEvent OnConnected;
		public event SocketServer<T, TSendPacket, TReceivePacket>.SocketReceiveEvent OnDataReceived;
		#endregion

		#region Constructors
		internal SocketClient(SocketServer<T, TSendPacket, TReceivePacket> Server, Socket socket, IPacketDecoder<T, TSendPacket, TReceivePacket> decoder, IPacketEncoder<T, TSendPacket, TReceivePacket> encoder)
		{
			this.Server = Server;
			this.Client = socket;
			this.NetworkStream = new SocketStream<T, TSendPacket, TReceivePacket>(this);
			this.Decoder = decoder;
			this.Encoder = encoder;
			IsClosed = false;
			IsThrowProtectEnable = Server.IsThrowProtectEnable;
		}

		internal SocketClient(SocketClient<T, TSendPacket, TReceivePacket> args)
		{
			//this.Buffer  = args.Buffer;
			this.Client = args.Client;
			this.NetworkStream = args.NetworkStream;
			this.ID = args.ID;
			this.Decoder = args.Decoder;
			this.Encoder = args.Encoder;
		}

		public SocketClient()
		{
			Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//Activator = new SimpleActivator<T, TSendPacket, TReceivePacket>();

			IsClosed = false;
			IsThrowProtectEnable = true;
		}
		#endregion

		#region InternalMethods
		internal void CallConnected(object sender, SocketConnectEventArgs<T, TSendPacket, TReceivePacket> args)
		{

			if(OnConnected != null)
			{
				OnConnected(sender, args);
			}
		}
		internal void CallDataReceived(object sender, SocketReceiveEventArgs<T, TSendPacket, TReceivePacket> args)
		{
			if(OnDataReceived != null)
			{
				OnDataReceived(sender, args);
			}
		}
		internal void CallSocketException(object sender, SocketErrorEventArgs<T, TSendPacket, TReceivePacket> args)
		{
			if(OnSocketException != null)
			{
				OnSocketException(sender, args);
			}
		}
		internal void CallDisconnect(object sender, SocketDisconnectEventArgs<T, TSendPacket, TReceivePacket> args)
		{
			if(OnDisconnect != null)
			{
				OnDisconnect(sender, args);
			}
		}

		#endregion

		#region PublicMethods
		public void Connect(string ip, int port)
		{
			Connect(new DnsEndPoint(ip, port));
		}
		public void Connect(EndPoint endp)
		{
			ConnectAsync(endp);
			if(!clientDone.WaitOne(3000))
				throw new Exception();
		}
		public void ConnectAsync(string ip, int port)
		{
			ConnectAsync(new DnsEndPoint(ip, port));
		}
		public void ConnectAsync(EndPoint endp)
		{
			if(Encoder == null)
				throw new InvalidOperationException("Encoderが未設定です。");
			if(Decoder == null)
				throw new InvalidOperationException("Decoderが未設定です。");

			Client.BeginConnect(endp, CollbackOnConnected, null);
		}
		public void Send(TSendPacket data)
		{
			if(Server != null)
				Server.Send(this, data);
			else
			{

				byte[] byteData = Encoder.Encode(data, this);

				if(Crypter != null)
				{
					byteData = Crypter.Encrypt(byteData, 0, byteData.Length);
				}

				Client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, OnSend, null);
			}
		}

		public void RawDataSend(byte[] data)
		{
			if(Server != null)
				Server.RawDataSend(this, data);
			else
			{
				if(Crypter != null)
				{
					data = Crypter.Encrypt(data, 0, data.Length);
				}

				Client.BeginSend(data, 0, data.Length, SocketFlags.None, OnSend, null);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="Exception">切断に失敗した時</exception>
		public void Close()
		{
			if(Client.Connected)
			{
				//Client.Disconnect(true);
				//Client.Shutdown(SocketShutdown.Both);

			}
			Client.Dispose();
			IsClosed = true;
		}






		public byte[] Receive(int size)
		{
			int now = 0;
			byte[] result = new byte[size];

			do
			{
				now += Client.Receive(result, now, size - now, SocketFlags.None);
			}
			while(now < size);


			if(now != size)
				throw new Exception();
			return result;
		}

		public void ReceiveToStream(Stream outstr, int size)
		{
			byte[] data = Receive(size);

			long pos = outstr.Position;

			//outstr.Seek(0, SeekOrigin.End);

			outstr.Write(data, 0, data.Length);

			outstr.Seek(pos, SeekOrigin.Begin);
		}
		#endregion

		#region CallbackMethods
		private void CollbackOnConnected(IAsyncResult ar)
		{
			var args = new SocketConnectEventArgs<T, TSendPacket, TReceivePacket>(this);
			this.NetworkStream = new SocketStream<T, TSendPacket, TReceivePacket>(this);
			try
			{
				Client.EndConnect(ar);

				clientDone.Set();
				if(State == null && Activator != null)
				{
					State = Activator.Activate(args);
				}

				CallConnected(this, args);

				IsClosed = false;
				byte[] Buffer = new byte[0];
				Client.BeginReceive(Buffer, 0, 0, SocketFlags.None, ReceiveTask, null);
			}
			catch(SocketException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Connect);
			}
			catch(ObjectDisposedException)
			{

			}
		}
		private void CallbackOnDisconnect(IAsyncResult ar)
		{
			Client.EndDisconnect(ar);


			var args = new SocketDisconnectEventArgs<T, TSendPacket, TReceivePacket>(this);
			CallDisconnect(this, args);
			clientDone.Set();
		}


		private void ReceiveTask(IAsyncResult ar)
		{

			var args = new SocketReceiveEventArgs<T, TSendPacket, TReceivePacket>(this);
			try
			{

				int len = Client.EndReceive(ar);



				if(this.Client.Available > 0)
				{
					//Console.WriteLine(len);
					lock(NetworkLock)
					{
						/*
							byte[] buff = Buffer;
							Buffer = new byte[len];
							System.Buffer.BlockCopy(buff, 0, Buffer, 0, len);
							if (Crypter!= null)
							{
								Buffer= Crypter.Decrypt(Buffer);
							}*/


						while(this.Client.Available > 0)
						{
							args.Packet = (TReceivePacket)Decoder.Decode(this, this, this.NetworkStream);
							UsingBuffer = NetworkStream.UsingBuffer.ToArray();
							if(OnDataReceived != null)
							{
								OnDataReceived(this, args);
							}
							NetworkStream.Flush();
							//CallDataReceived(this, args);
						}

					}

					byte[] Buffer = new byte[0];
					Client.BeginReceive(Buffer, 0, 0, SocketFlags.None, ReceiveTask, null);
				}


			}
			catch(InvalidProtocolException)
			{
				Close();
			}
			catch(SocketException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Receive);
			}
			catch(ObjectDisposedException)
			{
				Close();
			}

		}

		private void OnSend(IAsyncResult ar)
		{

			try
			{
				Client.EndSend(ar);
			}
			catch(SocketException ex)
			{
				this.SocketErrorCall(ex, new SocketEventArgs<T, TSendPacket, TReceivePacket>(this), SocketErrorType.Send);
			}
			catch(ObjectDisposedException)
			{

			}
		}


		#endregion

		#region PrivateMethods
		private void SocketErrorCall(SocketException ex, SocketEventArgs<T, TSendPacket, TReceivePacket> socket, SocketErrorType type)
		{
			var eArgs = new SocketErrorEventArgs<T, TSendPacket, TReceivePacket>(ex, socket, type);
			if(OnSocketException != null)
			{
				OnSocketException(this, eArgs);
			}
			//socket.Client.CallSocketException(this, eArgs);

			if(!socket.Client.Connected)
			{
				var args = new SocketDisconnectEventArgs<T, TSendPacket, TReceivePacket>(socket);

				if(OnDisconnect != null)
				{
					OnDisconnect(this, args);
				}
				socket.Client.CallDisconnect(this, args);
			}
			if(IsThrowProtectEnable == false)
				throw ex;
		}

		#endregion
	}
}
