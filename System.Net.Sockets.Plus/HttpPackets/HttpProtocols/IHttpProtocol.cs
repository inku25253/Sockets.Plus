using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpProtocols
{
	public interface IHttpProtocol
	{
		string Version { get; }

		string Encode(HttpSendPacket packet);
		HttpReceivePacket Decode();

	}
}
