using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{

	public class SocketReceiveEventArgs<T, TP> : SocketReceiveEventArgs<T, TP, TP>
	{
		public SocketReceiveEventArgs(SocketClient<T, TP, TP> client, byte[] buff = null)
			: base(client, buff)
		{
		}
	}
	public class SocketReceiveEventArgs<T, TSendPacket, TReceivePacket> : SocketEventArgs<T, TSendPacket, TReceivePacket>
	{
		public TReceivePacket Packet { get; internal set; }

		public byte[] ReceiveBuffer { get; private set; }

		public SocketReceiveEventArgs(SocketClient<T, TSendPacket, TReceivePacket> client, byte[] buff = null)
			: base(client)
		{
			this.ReceiveBuffer = buff;
		}
		public SocketReceiveEventArgs(SocketReceiveEventArgs<T, TSendPacket, TReceivePacket> args)
			: base(args.Client)
		{
			this.Packet = args.Packet;
		}


	}
}
