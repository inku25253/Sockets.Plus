using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.BytePackets
{
	public class ByteDecoder<T> : IPacketDecoder<T, BytePacket, BytePacket>
	{

		#region IPacketDecoder<T,BytePacket> メンバー

		public BytePacket Decode(object sender, SocketClient<T, BytePacket, BytePacket> client, SocketStream<T, BytePacket, BytePacket> net)
		{
			byte[] data = new byte[client.Client.Available];
			net.Read(data, 0, data.Length);

			return new BytePacket(data);
		}

		#endregion


	}
}
