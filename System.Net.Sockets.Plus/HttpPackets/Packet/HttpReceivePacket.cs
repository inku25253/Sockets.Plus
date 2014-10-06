using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpReceivePacket
	{
		public const string RequestHeaderPattern= @"(?<method>[\w]+)\s(?<path>[\w/%.=?&_-]+)\sHTTP/(?<version>[0-9\.]+)";
		public const string PagePattern			= @"/([\w%.=?&_-]+)?";
		public const string NoArgsPagePattern	= @"/[\w%.]+";
		public const string PageArgumentPattern	= @"[?&](?<argument>[\w%._-]+)([=]?(?<value>[\w%._-]+)?)";


		internal static HttpReceivePacket ParseRequest(string sHttpRequest)
		{
			string[] header = sHttpRequest.Split('\n');

			Regex regex = new Regex(RequestHeaderPattern);

			Match match = regex.Match(header[0]);
			if (match.Success == false)
				throw new InvalidProtocolException();


			HttpReceivePacket result = new HttpReceivePacket();
			result.Method = match.Groups["method"].Value;
			result.Path = HttpPath.Parse(match.Groups["path"].Value);
			result.Version = "HTTP/"+match.Groups["version"].Value;





			return result;


		}
		private HttpReceivePacket()
		{
			this.Version  = "HTTP/1.1";
		}


		public string Method;
		public HttpPath Path;
		public string Version;






	}
}
