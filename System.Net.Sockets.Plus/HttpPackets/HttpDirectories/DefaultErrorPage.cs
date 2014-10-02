using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpDirectories
{
	public class DefaultErrorPage : IPage
	{
		public HttpStatus Status;
		public DefaultErrorPage(HttpStatus status)
		{
			this.Status =status;
		}
		public override HttpSendPacket PageRead(HttpPath Args)
		{

			string html = string.Format(
			"<!DOCTYPE html\">"+
			"<html>"+
			"<head>"+
			"<title>Error</title>"+
			"</head>"+
			"<body>"+
			"<font size = 16>"+
			"<center>"+
			"<br>"+
			"<table>"+
			"<tr> ERROR!! </tr><tr>  </tr><tr> {0} {1} </tr>"+
			"</table>"+
			"</center>"+
			"</font>"+
			"</body>"+
			"</html>", (int)Status, Enum.GetName(typeof(HttpStatus), Status));
			return new HttpSendPacket(Status, html, Encode);
		}
	}
}
