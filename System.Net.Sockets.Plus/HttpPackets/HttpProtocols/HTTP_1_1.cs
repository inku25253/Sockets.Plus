using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpProtocols
{
	public class HTTP_1_1 : IHttpProtocol
	{
		#region IHttpProtocol メンバー

		public string Version
		{
			get { return "HTTP/1.1"; }
		}
		public HttpReceivePacket Decode()
		{
			return null;
		}

		#endregion

		#region IHttpProtocol メンバー


		public string Encode(HttpSendPacket packet)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
