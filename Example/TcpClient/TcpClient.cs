using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets.Plus;
using System.Net.Sockets.Plus.BytePackets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClient
{
	public class TcpClient
	{

		SocketClient client;


		public TcpClient()
		{
			client = new SocketClient();




			//イベント登録/////
			{
				//接続イベント
				client.OnConnected			+=client_OnConnected;
				//データ受信イベント
				client.OnDataReceived		+=client_OnDataReceived;
				//切断イベント
				client.OnDisconnect			+=client_OnDisconnect;
				//エラーイベント
				client.OnSocketException	+=client_OnSocketException;
			}
			///////////////////

			client.Connect("127.0.0.1", TcpServer.Port);
		}
		~TcpClient()
		{
			client.Close();
			client = null;
		}

		public void Send(string data)
		{
			client.Send(new BytePacket(data));
		}





		void client_OnConnected(object sender, SocketConnectEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine(args.ToString());

			args.Send(new BytePacket("Hello"));
		}
		void client_OnDataReceived(object sender, SocketReceiveEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine(args.Packet.ToUTF8());
		}
		void client_OnDisconnect(object sender, SocketDisconnectEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine(args.ToString());
		}
		void client_OnSocketException(object sender, SocketErrorEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine("Exception: {0}=> {1}", args.Type, args.Exception.Message);
		}
	}
}
