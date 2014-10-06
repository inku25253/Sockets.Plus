using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets.Plus;
using System.Text;
using System.Threading.Tasks;

namespace TcpClient
{
	class Program
	{
		static void Main(string[] args)
		{
			bool isServerMode = false;
			TcpServer server = null;
			TcpClient client = null;
			if (SocketServer.CheckPort(TcpServer.Port))
			{
				server = new TcpServer();
				isServerMode = true;
			}
			else
			{
				client = new TcpClient();
				isServerMode = false;
			}

			string text =Console.ReadLine();
			while (text != "quit")
			{
				if (text == "\\n")
					text = "\n";
				if (isServerMode == false)
				{

					client.Send(text);


				}
				else
				{
					server.Send(text);
				}


				text = Console.ReadLine();
			}


			server = null;
			client = null;
		}

	}
}
