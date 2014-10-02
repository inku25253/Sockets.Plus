using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets.Plus.HttpPackets.HttpDirectories;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets
{
	public class HttpPath
	{

		public string RequestPath { get; private set; }
		public string Path { get; private set; }
		public Dictionary<string, string> Argument { get; private set; }

		public HttpPath Next { get; private set; }
		public HttpPath Prev { get; private set; }


		public HttpPath()
		{
			Argument = new Dictionary<string, string>();
		}
		public string this[string key]
		{
			get { return Argument[key]; }
		}
		public bool Contains(string key)
		{
			return Argument.ContainsKey(key);
		}



		internal static HttpPath PathCheck(string page)
		{


			HttpPath result = new HttpPath();
			Regex regex = new Regex(HttpReceivePacket.PageArgumentPattern);

			foreach (Match argumentMatch in regex.Matches(page))
			{
				if (argumentMatch.Success)
				{
					result.Argument.Add(argumentMatch.Groups["argument"].Value, argumentMatch.Groups["value"].Value);
				}
			}


			regex = new Regex(HttpReceivePacket.NoArgsPagePattern);
			Match pageMatch = regex.Match(page);


			result.Path = pageMatch.Value;

			result.RequestPath = page;

			return result;
		}

		internal static HttpPath Parse(string request)
		{

			HttpPath result = null;

			Regex regex = new Regex(HttpReceivePacket.PagePattern);
			HttpPath old = null;
			foreach (Match match in regex.Matches(request))
			{
				if (match.Success)
				{
					if (old == null)
					{
						old = HttpPath.PathCheck(match.Value);

						result = old;
					}
					else
					{
						HttpPath path = HttpPath.PathCheck(match.Value);
						old.Next = path;

						path.Prev = old;

						old = old.Next;


					}
				}
			}
			return result;
		}
	}
}
