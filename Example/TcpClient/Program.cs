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
			TcpServer server;
			TcpClient client;
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




		}

	}
}
