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

	public class SocketClient : SocketClient<object>
	{
		public SocketClient()
			: base()
		{

		}
	}
	public class SocketClient<T> : SocketClient<T, BytePacket>
	{
		public SocketClient()
		{
			this.Decoder = new ByteDecoder<T>();
			this.Encoder = new ByteEncoder<T>();

		}
	}


	public class SocketClient<T, TP>
	{
		#region Fields
		private ManualResetEvent clientDone = new ManualResetEvent(false);
		#endregion
		#region Properties
		//private int bufferSize = 8192;
		//public int BufferSize { get { return bufferSize; } set { bufferSize = value; } }
		public byte[] UsingBuffer { get; set; }

		private readonly object NetworkLock = new object();

		public Socket Client;
		public SocketServer<T, TP> Server { get; private set; }
		public T State { get; set; }

		public int ID { get; internal set; }


		public bool Connected { get { return Client.Connected; } }

		public SocketStream<T, TP> NetworkStream { get; set; }

		public IPacketDecoder<T, TP> Decoder { get; set; }
		public IPacketEncoder<T, TP> Encoder { get; set; }

		public IPacketCrypter Crypter { get; set; }

		public bool IsClosed { get; private set; }
		#endregion

		#region Events
		public event SocketServer<T, TP>.SocketEvent OnDisconnect;
		public event SocketServer<T, TP>.SocketErrorEvent OnSocketException;
		public event SocketServer<T, TP>.SocketEvent OnConnected;
		public event SocketServer<T, TP>.SocketEvent OnDataReceived;
		#endregion

		#region Constructors
		internal SocketClient(SocketServer<T, TP> Server, Socket socket, IPacketDecoder<T, TP> decoder, IPacketEncoder<T, TP> encoder)
		{
			this.Server = Server;
			this.Client = socket;
			this.NetworkStream = new SocketStream<T, TP>(this);
			this.Decoder = decoder;
			this.Encoder = encoder;
			IsClosed = false;
		}

		internal SocketClient(SocketClient<T, TP> args)
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

		}
		#endregion

		#region InternalMethods
		internal void CallConnected(object sender, SocketEventArgs<T, TP> args)
		{
			if (OnConnected != null)
			{
				OnConnected(sender, args);
			}
		}
		internal void CallDataReceived(object sender, SocketEventArgs<T, TP> args)
		{
			if (OnDataReceived != null)
			{
				OnDataReceived(sender, args);
			}
		}
		internal void CallSocketException(object sender, SocketErrorEventArgs<T, TP> args)
		{
			if (OnSocketException != null)
			{
				OnSocketException(sender, args);
			}
		}
		internal void CallDisconnect(object sender, SocketEventArgs<T, TP> args)
		{
			if (OnDisconnect != null)
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
			if (!clientDone.WaitOne(3000))
				throw new Exception();
		}
		public void ConnectAsync(string ip, int port)
		{
			ConnectAsync(new DnsEndPoint(ip, port));
		}
		public void ConnectAsync(EndPoint endp)
		{
			if (Encoder == null)
				throw new InvalidOperationException("Encoderが未設定です。");
			if (Decoder == null)
				throw new InvalidOperationException("Decoderが未設定です。");

			Client.BeginConnect(endp, CollbackOnConnected, null);
		}
		public void Send(TP data)
		{
			if (Server != null)
				Server.Send(this, data);
			else
			{

				byte[] byteData = Encoder.Encode(data, this);

				if (Crypter != null)
				{
					byteData = Crypter.Encrypt(byteData, 0, byteData.Length);
				}

				Client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, OnSend, null);
			}
		}

		public void Send(byte[] data)
		{
			if (Server != null)
				Server.Send(this, data);
			else
			{
				if (Crypter != null)
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
			if (Client.Connected)
			{
				Client.Shutdown(SocketShutdown.Both);

				Client.Disconnect(true);
			}
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
			while (now < size);


			if (now != size)
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
			SocketEventArgs<T, TP> args = new SocketEventArgs<T, TP>(this);
			this.NetworkStream = new SocketStream<T, TP>(this);
			try
			{
				Client.EndConnect(ar);

				CallConnected(this, args);

				clientDone.Set();
				IsClosed = false;
				byte[] Buffer = new byte[0];
				Client.BeginReceive(Buffer, 0, 0, SocketFlags.None, ReceiveTask, null);
			}
			catch (SocketException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Connect);
			}
		}
		private void CallbackOnDisconnect(IAsyncResult ar)
		{
			Client.EndDisconnect(ar);


			SocketEventArgs<T, TP> args = new SocketEventArgs<T, TP>(this);
			CallDisconnect(this, args);
			clientDone.Set();
		}


		private void ReceiveTask(IAsyncResult ar)
		{

			SocketEventArgs<T, TP> args = new SocketEventArgs<T, TP>(this);
			try
			{

				int len = Client.EndReceive(ar);



				if (this.Client.Available > 0)
				{
					//Console.WriteLine(len);
					lock (NetworkLock)
					{
						/*
							byte[] buff = Buffer;
							Buffer = new byte[len];
							System.Buffer.BlockCopy(buff, 0, Buffer, 0, len);
							if (Crypter!= null)
							{
								Buffer= Crypter.Decrypt(Buffer);
							}*/


						while (this.Client.Available > 0)
						{
							args.Packet =(TP)Decoder.Decode(this, this, this.NetworkStream);
							UsingBuffer = NetworkStream.UsingBuffer.ToArray();
							if (OnDataReceived != null)
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
			catch (SocketException ex)
			{
				this.SocketErrorCall(ex, args, SocketErrorType.Receive);
			}
			catch (InvalidProtocolException)
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
			catch (SocketException ex)
			{
				this.SocketErrorCall(ex, new SocketEventArgs<T, TP>(this), SocketErrorType.Send);
			}
		}


		#endregion

		#region PrivateMethods
		private void SocketErrorCall(SocketException ex, SocketEventArgs<T, TP> socket, SocketErrorType type)
		{
			SocketErrorEventArgs<T, TP> eArgs = new SocketErrorEventArgs<T, TP>(ex, socket, type);
			if (OnSocketException != null)
			{
				OnSocketException(this, eArgs);
			}
			//socket.Client.CallSocketException(this, eArgs);

			if (!socket.Client.Connected)
			{
				SocketEventArgs<T, TP> args = new SocketEventArgs<T, TP>(socket);

				if (OnDisconnect != null)
				{
					OnDisconnect(this, args);
				}
				socket.Client.CallDisconnect(this, args);
			}
		}

		#endregion
	}
}
