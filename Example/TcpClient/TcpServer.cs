using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets.Plus;
using System.Net.Sockets.Plus.BytePackets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClient
{
	public class TcpServer
	{



		public const int Port = 990;

		SocketServer Server;

		public TcpServer()
		{
			Server = new SocketServer();




			bool isSuccess = Server.Setup(Port);

			if (isSuccess == false)
			{
				Console.WriteLine("SetupError: {0}", Server.LastError.Message);
			}


			//イベント登録///////
			{
				//接続要求イベント
				Server.OnConnectRequest		+=Server_OnConnectRequest;
				//データ受信イベント
				Server.OnDataReceived		+=Server_OnDataReceived;
				//切断時イベント
				Server.OnDisconnect			+=Server_OnDisconnect;
				//エラー発生イベント
				Server.OnSocketException	+=Server_OnSocketException;
			}
			/////////////////////

			Server.Start();
		}
		~TcpServer()
		{
			Server.Stop();
			Server = null;
		}

		public void Send(string text)
		{
			Server.AllClientSend(new BytePacket(text));
		}


		void Server_OnConnectRequest(object sender, SocketConnectEventArgs<object, BytePacket, BytePacket> args)
		{

			Console.WriteLine(args.ToString());

		}
		void Server_OnDataReceived(object sender, SocketReceiveEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine(args.Packet.ToUTF8());
			args.Server.AllClientSend(args.Packet);


		}
		void Server_OnDisconnect(object sender, SocketDisconnectEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine(args.ToString());
		}

		void Server_OnSocketException(object sender, SocketErrorEventArgs<object, BytePacket, BytePacket> args)
		{
			Console.WriteLine("Exception: {0}=> {1}", args.Type, args.Exception.Message);
		}



	}
}
