using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.BytePackets
{
	public class ByteEncoder<T> : IPacketEncoder<T, BytePacket>
	{
		#region IPacketEncoder<T,ByteEncoder<T>> メンバー

		public byte[] Encode(BytePacket obj, SocketClient<T, BytePacket> client)
		{
			return obj.Buffer;
		}

		#endregion
	}
}
