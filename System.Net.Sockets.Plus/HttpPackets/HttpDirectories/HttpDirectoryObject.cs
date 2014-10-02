using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpDirectories
{
	public abstract class HttpDirectoryObject
	{
		public abstract HttpSendPacket PageRead(HttpPath Args);


		public Encoding Encode = new UTF8Encoding();
		public HttpSendPacket OK(string html)
		{
			return new HttpSendPacket(HttpStatus.OK, html, Encode);
		}
		public HttpSendPacket NotFound(string html)
		{
			return new HttpSendPacket(HttpStatus.NotFound, html, Encode);
		}
	}
}
