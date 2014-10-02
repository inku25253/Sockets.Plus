using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets.Plus;
using System.Net.Sockets.Plus.BytePackets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
	class Program
	{
		static Stopwatch sw;
		static TimeSpan sendTest = new TimeSpan(0);
		static ManualResetEvent done = new ManualResetEvent(false);
		static int count = 0, oServerStart = 0, oClientStart = 0, oSend = 0;
		static TimeSpan oneMilliseconds = new TimeSpan(0, 0, 0, 0, 1);
		static void Main(string[] args)
		{

			TimeSpan serverStart = new TimeSpan(0), clientStart = new TimeSpan(0);

			Random rand = new Random();

			sw = new Stopwatch();

			while (count < 10000)
			{
				done.Reset();

				Console.SetCursorPosition(0, 0);
				sw.Restart();

				SocketServer server = new SocketServer();

				server.Throwable_Setup(new IPEndPoint(IPAddress.Any, 990));


				server.OnConnectRequest +=server_OnConnectRequest;
				server.OnDataReceived +=server_OnDataReceived;
				server.OnDisconnect +=server_OnDisconnect;
				server.Start();
				sw.Stop();
				//Console.WriteLine("Server StartTime: {0}", sw.Elapsed);
				if (sw.Elapsed < oneMilliseconds)
					++oServerStart;
				serverStart += sw.Elapsed;
				sw.Restart();

				SocketClient client = new SocketClient();
				client.OnConnected +=client_OnConnected;
				client.OnDataReceived += client_OnDataReceived;
				client.OnDisconnect +=client_OnDisconnect;
				client.Connect("127.0.0.1", 990);

				sw.Stop();
				//Console.WriteLine("Client StartTime: {0}", sw.Elapsed);
				if (sw.Elapsed < oneMilliseconds)
					++oClientStart;
				clientStart += sw.Elapsed;

				sw.Restart();
				byte[] sendData = new byte[1024];
				rand.NextBytes(sendData);
				client.Send(sendData);

				//Console.ReadLine();
				done.WaitOne();

				++count;
				TimeSpan current;
				/*
				Console.WriteLine("=====平均=====                                               ");

				Console.WriteLine("ServerStart: {0}", new TimeSpan(serverStart.Ticks / count));
				Console.WriteLine("ClientStart: {0}", new TimeSpan(clientStart.Ticks / count));
				Console.WriteLine("SendTest   : {0}", new TimeSpan(sendTest.Ticks / count));
				Console.WriteLine("=====平均=====                                               ");
				Console.WriteLine();
				Console.WriteLine("Count: {0:000}", count);
				Console.WriteLine();
				Console.WriteLine("=====1ms以上かかった回数=====                                ");
				Console.WriteLine("ServerStart: {0}", oServerStart);
				Console.WriteLine("ClientStart: {0}", oClientStart);
				Console.WriteLine("SendTest   : {0}", oSend);
				Console.WriteLine("=====1ms以上かかった回数=====                                ");*/
				client.Close();
				server.Stop();

				//Thread.Sleep(100);
			}

			StreamWriter writer = new StreamWriter("log.log");
			writer.WriteLine("=====平均=====                                               ");

			writer.WriteLine("ServerStart: {0}", new TimeSpan(serverStart.Ticks / count));
			writer.WriteLine("ClientStart: {0}", new TimeSpan(clientStart.Ticks / count));
			writer.WriteLine("SendTest   : {0}", new TimeSpan(sendTest.Ticks / count));
			writer.WriteLine("=====平均=====                                               ");
			writer.WriteLine();
			writer.WriteLine("=====1ms以上かかった回数=====                                ");
			writer.WriteLine("ServerStart: {0}({1:p})", oServerStart, (oServerStart / 10000) * 100);
			writer.WriteLine("ClientStart: {0}({1:p})", oClientStart, (oClientStart / 10000) * 100);
			writer.WriteLine("SendTest   : {0}({1:p})", oSend, (oSend / 10000) * 100);
			writer.WriteLine("=====1ms以上かかった回数=====                                ");
			writer.Flush();
			writer.Close();
			writer.Dispose();
		}

		private static void client_OnDisconnect(object sender, SocketEventArgs<object, BytePacket, BytePacket> args)
		{
		}

		private static void client_OnDataReceived(object sender, SocketEventArgs<object, BytePacket, BytePacket> args)
		{
		}

		private static void client_OnConnected(object sender, SocketEventArgs<object, BytePacket, BytePacket> args)
		{
		}

		private static void server_OnDisconnect(object sender, SocketEventArgs<object, BytePacket, BytePacket> args)
		{
		}

		private static void server_OnDataReceived(object sender, SocketEventArgs<object, BytePacket, BytePacket> args)
		{
			sw.Stop();

			//	Console.WriteLine("Server  SendTime: {0}", sw.Elapsed);
			if (sw.Elapsed < oneMilliseconds)
				++oSend;
			sendTest += sw.Elapsed;
			done.Set();
		}

		private static void server_OnConnectRequest(object sender, SocketEventArgs<object, BytePacket, BytePacket> args)
		{
		}


		static void client_OnDataReceived(object sender, SocketEventArgs<object, System.Net.Sockets.Plus.BytePackets.BytePacket> args)
		{
			sw.Stop();

			//	Console.WriteLine("Client: {0}", sw.Elapsed);
		}

	}
}
