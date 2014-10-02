using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpEncoder : IPacketEncoder<HttpClient, HttpSendPacket, HttpReceivePacket>
	{
		#region IPacketEncoder<HttpClient,HttpPacket> メンバー

		public byte[] Encode(HttpSendPacket packet, SocketClient<HttpClient, HttpSendPacket, HttpReceivePacket> client)
		{
			StringBuilder result = new StringBuilder();

			result.AppendFormat("{0} {1} {2}\r\n", packet.Version, packet.StatusCode, packet.StatusName);
			result.AppendLine("Content-Length: "+packet.Encode.GetByteCount(packet.HTML));
			result.AppendLine();
			result.AppendLine(packet.HTML);
			return packet.Encode.GetBytes(result.ToString());
		}

		#endregion

	}
}
