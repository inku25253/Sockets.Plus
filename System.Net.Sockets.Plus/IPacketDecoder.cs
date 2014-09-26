using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public interface IPacketDecoder<T, TP>
	{
		TP Decode(object sender, SocketClient<T, TP> client, SocketStream<T, TP> stream);
	}
}
