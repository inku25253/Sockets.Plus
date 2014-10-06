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
			Random rand = new Random();
			string[] omikuzi = new string[] { "大吉", "中吉", "小吉", "末吉", "吉" };
			public override HttpSendPacket PageRead(HttpPath Args)
			{
				int randomValue = rand.Next(omikuzi.Length);
				string omikuziResult = omikuzi[randomValue];
				return IAmTeapot(string.Format("{0} ({1})", omikuziResult, randomValue));
			}
		}
	}
}
