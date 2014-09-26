using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public class SocketEventArgs<T, TP> : EventArgs
	{
		public SocketClient<T, TP> Client { get; protected set; }
		public TP Packet { get; internal set; }

		public byte[] ReceiveBuffer { get; private set; }

		public SocketEventArgs(SocketClient<T, TP> client, byte[] buff = null)
		{
			this.Client = client;
			this.ReceiveBuffer = buff;
		}
		public SocketEventArgs(SocketEventArgs<T, TP> args)
		{
			this.Client = args.Client;
			this.Packet = args.Packet;
		}

		public void Send(TP tp)
		{
			Client.Send(tp);
		}


	}
}
