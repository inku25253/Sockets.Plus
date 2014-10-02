using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets.Plus.HttpPackets;
using System.Net.Sockets.Plus.HttpPackets.HttpDirectories;
using System.Text;
using System.Threading.Tasks;

namespace ExampleHttpServer
{
	class Program
	{
		static void Main(string[] args)
		{
			HttpServer server = new HttpServer();
			server.Start(new IPEndPoint(IPAddress.Any, 980));

			server.DirectoryService.Directory["omikuzi"] = new testPage();


			Console.ReadLine();
		}
		public class testPage : IPage
		{
			public override HttpSendPacket PageRead(HttpPath Args)
			{
				return OK("OK!!!");
			}
		}
	}
}
