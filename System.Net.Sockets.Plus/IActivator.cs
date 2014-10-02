using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public interface IActivator<TState, TPacket> : IActivator<TState, TPacket, TPacket> { }
	public interface IActivator<TState, TSendPacket, TReceivePacket>
	{
		TState Activate(SocketEventArgs<TState, TSendPacket, TReceivePacket> args);
	}
}
