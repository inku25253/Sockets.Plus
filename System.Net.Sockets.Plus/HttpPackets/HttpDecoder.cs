using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpDecoder : IPacketDecoder<HttpClient, HttpSendPacket, HttpReceivePacket>
	{
		#region IPacketDecoder<HttpClient,HttpPacket> メンバー

		public HttpReceivePacket Decode(object sender, SocketClient<HttpClient, HttpSendPacket, HttpReceivePacket> client, SocketStream<HttpClient, HttpSendPacket, HttpReceivePacket> stream)
		{


			byte[] datas = stream.ReadAll();


			string sHttpRequest = Encoding.UTF8.GetString(datas);

			try
			{
				HttpReceivePacket result = HttpReceivePacket.ParseRequest(sHttpRequest);
				return result;

			}
			catch (InvalidProtocolException)
			{

				HttpSendPacket error_Packet = new HttpSendPacket(HttpStatus.BadRequest, "<font size=16>BadRequest</font>", Encoding.UTF8);
				client.Send(error_Packet);
				client.Close();
				return null;

			}


		}

		#endregion
	}
}
