using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpSendPacket
	{
		public HttpSendPacket(HttpStatus status)
			: this((int)status, Enum.GetName(typeof(HttpStatus), status))
		{

		}
		public HttpSendPacket(HttpStatus status, string html, Encoding encode)
			: this((int)status, Enum.GetName(typeof(HttpStatus), status))
		{
			this.Encode = encode;
			this.HTML = html;

		}
		public HttpSendPacket(int code, string name)
			: this()
		{
			this.StatusCode = code;
			this.StatusName = name;
		}
		public HttpSendPacket()
		{
			this.Version  = "HTTP/1.1";
		}

		public Encoding Encode;

		public string Version;
		public int StatusCode;
		public string StatusName;

		public string HTML;
	}
}
