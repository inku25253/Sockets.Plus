using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{

	public interface IPacketEncoder<T, TSendPacket, TReceivePacket>
	{
		byte[] Encode(TSendPacket packet, SocketClient<T, TSendPacket, TReceivePacket> client);

	}
}
