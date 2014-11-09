using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets.Plus.HttpPackets.HttpDirectories;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpServer : HttpClient
	{

		SocketServer<HttpClient, HttpSendPacket, HttpReceivePacket> Server;


		public HttpDirectoryService DirectoryService { get; private set; }

		public HttpServer()
		{
			Server = new SocketServer<HttpClient, HttpSendPacket, HttpReceivePacket>();
			Server.DefaultDecoder = new HttpDecoder();
			Server.DefaultEncoder = new HttpEncoder();
			Server.Activator = new SimpleActivator<HttpClient, HttpSendPacket, HttpReceivePacket>(typeof(SocketClientRequest), this);
			this.DirectoryService = new HttpDirectoryService();







			Server.OnDataReceived +=Server_OnDataReceived;





		}

		void Server_OnDataReceived(object sender, SocketReceiveEventArgs<HttpClient, HttpSendPacket, HttpReceivePacket> args)
		{
			IPage page = this.DirectoryService.GetPage(args.Packet.Path);

			HttpSendPacket packet;
			if (page == null)
				packet = this.DirectoryService.GetErrorPage(HttpStatus.NotFound).PageRead(null);
			else packet = page.PageRead(args.Packet.Path);
			args.Send(packet);
			args.Client.Close();
		}

		public void Start(EndPoint endp)
		{
			Server.Throwable_Setup(endp);
			Server.Start();
		}





	}
}
