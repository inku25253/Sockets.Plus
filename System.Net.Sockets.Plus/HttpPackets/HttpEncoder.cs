using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpEncoder : IPacketEncoder<HttpClient, HttpPacket>
	{
		#region IPacketEncoder<HttpClient,HttpPacket> メンバー

		public byte[] Encode(HttpPacket obj, SocketClient<HttpClient, HttpPacket> client)
		{
			return new byte[0];
		}

		#endregion
	}
}
