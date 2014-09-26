using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.BytePackets
{
	public class BytePacket
	{
		public byte[] Buffer { get; set; }
		public BytePacket(byte[] buffer)
		{
			this.Buffer = buffer;
		}
		public BytePacket(string str)
		{
			Buffer= Encoding.UTF8.GetBytes(str);
		}
		public string ToUTF8()
		{
			return Encoding.UTF8.GetString(Buffer);
		}
	}
}
