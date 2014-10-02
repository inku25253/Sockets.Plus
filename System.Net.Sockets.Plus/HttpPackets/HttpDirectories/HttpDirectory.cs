using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpDirectories
{
	public class HttpDirectory : IHttpDirectory
	{
		public override HttpSendPacket PageRead(HttpPath Args)
		{
			return OK("");
		}
	}
}
