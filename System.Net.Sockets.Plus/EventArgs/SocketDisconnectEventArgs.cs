using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public class SocketDisconnectEventArgs<T, TSendPacket, TReceivePacket> : SocketConnectEventArgs<T, TSendPacket, TReceivePacket>
	{

		public SocketDisconnectEventArgs(SocketClient<T, TSendPacket, TReceivePacket> client)
			: base(client)
		{
		}
		public SocketDisconnectEventArgs(SocketEventArgs<T, TSendPacket, TReceivePacket> args)
			: base(args)
		{
		}


		public override string ToString()
		{
			IPEndPoint ipEndPoint = RemoteEndPoint as IPEndPoint;
			if (ipEndPoint != null)
				return string.Format("DISCONNECT: {0}", ipEndPoint.ToString());
			else return base.ToString();
		}

	}
}
