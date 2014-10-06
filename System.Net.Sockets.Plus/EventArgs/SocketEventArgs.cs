using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{

	public class SocketSendEventArgs<T, TSendPacket, TReceivePacket> : SocketEventArgs<T, TSendPacket, TReceivePacket>
	{
		public TSendPacket Packet { get; internal set; }


		public SocketSendEventArgs(SocketClient<T, TSendPacket, TReceivePacket> client)
			: base(client)
		{

		}
	}



	public class SocketEventArgs<T, TP> : SocketEventArgs<T, TP, TP>
	{
		public SocketEventArgs(SocketClient<T, TP, TP> client)
			: base(client)
		{
		}
		public SocketEventArgs(SocketEventArgs<T, TP, TP> args)
			: base(args)
		{
		}

	}
	public class SocketEventArgs<T, TSendPacket, TReceivePacket> : EventArgs
	{
		public SocketClient<T, TSendPacket, TReceivePacket> Client { get; protected set; }
		public SocketServer<T, TSendPacket, TReceivePacket> Server { get { return Client.Server; } }

		public SocketEventArgs(SocketClient<T, TSendPacket, TReceivePacket> client)
		{
			this.Client = client;

		}
		public SocketEventArgs(SocketEventArgs<T, TSendPacket, TReceivePacket> args)
		{
			this.Client = args.Client;
		}

		public void Send(TSendPacket tp)
		{
			Client.Send(tp);
		}


	}









}
