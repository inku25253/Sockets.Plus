using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{

	public interface IPacketDecoder<T, TSendPacket, TReceivePacket>
	{
		TReceivePacket Decode(
			object sender,
			SocketClient<T, TSendPacket, TReceivePacket> client,
			SocketStream<T, TSendPacket, TReceivePacket> stream
		);
	}
}
