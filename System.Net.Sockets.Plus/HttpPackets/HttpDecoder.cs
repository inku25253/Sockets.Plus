using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpDecoder : IPacketDecoder<HttpClient, HttpPacket>
	{
		#region IPacketDecoder<HttpClient,HttpPacket> メンバー

		public HttpPacket Decode(object sender, SocketClient<HttpClient, HttpPacket> client, SocketStream<HttpClient, HttpPacket> stream)
		{
			byte[] datas = stream.ReadAll();
			string sHttpRequest = Encoding.UTF8.GetString(datas);



			return HttpPacket.ParseRequest(sHttpRequest);
		}

		#endregion
	}
}
