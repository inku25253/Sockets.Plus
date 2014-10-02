using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{

	public class SocketStream<T, TP> : SocketStream<T, TP, TP>
	{
		public SocketStream(SocketClient<T, TP, TP> client) : base(client) { }
	}
	public class SocketStream<T, TSendPacket, TReceivePacket> : Stream
	{



		public MemoryStream UsingBuffer = new MemoryStream();
		public bool IsStreamMode;
		public SocketClient<T, TSendPacket, TReceivePacket> Client { get; set; }
		public SocketStream(SocketClient<T, TSendPacket, TReceivePacket> client)
		{
			this.Client = client;
		}




		public void BufferReceive(int len)
		{
			Client.ReceiveToStream(UsingBuffer, len);
		}


		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{

			UsingBuffer = new MemoryStream();
		}

		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}


		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (IsStreamMode)
			{
				return UsingBuffer.Read(buffer, offset, count);
			}
			else
			{
				int len =  Client.Client.Receive(buffer, offset, count, SocketFlags.None);
				if (Client.Crypter != null)
				{
					byte[] decrypt = Client.Crypter.Decrypt(buffer, offset, count);
					Buffer.BlockCopy(decrypt, 0, buffer, offset, decrypt.Length);

				}
				UsingBuffer.Write(buffer, offset, len);
				return len;

			}
		}
		public byte[] ReadAll()
		{
			int length = Client.Client.Available;
			byte[] result = new byte[length];
			int readLength = 0;
			do
			{
				readLength += Read(result, readLength, length-readLength);
			} while (readLength < length);

			return result;
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
