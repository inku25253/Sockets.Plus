using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpClient
	{


		SocketClient<HttpClient, HttpSendPacket, HttpReceivePacket> Client;
		public HttpServer Server { get; private set; }
		public HttpClient(SocketClient<HttpClient, HttpSendPacket, HttpReceivePacket> client, HttpServer server)
		{
			this.Client = client;
			this.Server = server;
		}
		public HttpClient()
		{

		}


		public void SendBadRequestError()
		{
			HttpSendPacket error_Packet = new HttpSendPacket(HttpStatus.BadRequest, "<font size=16>BadRequest</font>", Encoding.UTF8);
			Client.Send(error_Packet);
			Client.Close();
		}
	}
}
