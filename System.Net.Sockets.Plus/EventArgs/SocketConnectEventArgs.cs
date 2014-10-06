using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public class SocketConnectEventArgs<T, TSendPacket, TReceivePacket> : SocketEventArgs<T, TSendPacket, TReceivePacket>
	{

		public EndPoint RemoteEndPoint { get { return Client.Client.RemoteEndPoint; } }

		public SocketConnectEventArgs(SocketClient<T, TSendPacket, TReceivePacket> client)
			: base(client)
		{
		}
		public SocketConnectEventArgs(SocketEventArgs<T, TSendPacket, TReceivePacket> args)
			: base(args)
		{
		}


		public override string ToString()
		{
			try
			{
				IPEndPoint ipEndPoint = RemoteEndPoint as IPEndPoint;
				if (ipEndPoint != null && Client.Connected)
					return string.Format("CONNECT: {0}", ipEndPoint.ToString());
				else return base.ToString();
			}
			catch (SocketException)
			{
				return base.ToString();
			}
		}

	}
}
