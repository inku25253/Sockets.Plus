using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpServer : HttpClient
	{

		SocketServer<HttpClient, HttpPacket> Server;

		public HttpServer()
		{
			Server = new SocketServer<HttpClient, HttpPacket>();
			Server.DefaultDecoder = new HttpDecoder();
			Server.DefaultEncoder = new HttpEncoder();

			Server.OnConnectRequest +=Server_OnConnectRequest;
			Server.OnDataReceived +=Server_OnDataReceived;
			Server.OnDisconnect +=Server_OnDisconnect;
		}




		HttpClient Server_OnConnectRequest(object sender, SocketEventArgs<HttpClient, HttpPacket> args)
		{


			return new HttpClient();
		}
		void Server_OnDataReceived(object sender, SocketEventArgs<HttpClient, HttpPacket> args)
		{
			Console.WriteLine(args.Packet.Method);
			Console.WriteLine(args.Packet.Path);
			Console.WriteLine(args.Packet.Version);
		}
		void Server_OnDisconnect(object sender, SocketEventArgs<HttpClient, HttpPacket> args)
		{

		}


		public void Start(EndPoint endp)
		{
			Server.Throwable_Setup(endp);
			Server.Start();
		}



	}
}
