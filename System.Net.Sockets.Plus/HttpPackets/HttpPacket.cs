using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpPacket
	{
		public const string RequestHeaderPattern= @"(?<method>[\w]+)\s(?<path>[\w/%.=?&]+)\sHTTP/(?<version>[0-9\.]+)";
		public const string PagePattern			= @"/[\w%.=?&]+";
		public const string PageArgumentPattern	= @"[?&](?<argument>[\w%.]+)([=]?(?<value>[\w%.]+)?)";
		internal static HttpPacket ParseRequest(string sHttpRequest)
		{
			string[] header = sHttpRequest.Split('\n');

			Regex regex = new Regex(RequestHeaderPattern);

			Match match = regex.Match(header[0]);
			if (match.Success == false)
				throw new InvalidProtocolException();


			HttpPacket result = new HttpPacket();
			result.Method = match.Groups["method"].Value;
			result.Path = match.Groups["path"].Value;
			result.Version = "HTTP/"+match.Groups["version"].Value;

			return result;


		}





		public string Method;
		public string Path;
		public string Version;
	}
}
