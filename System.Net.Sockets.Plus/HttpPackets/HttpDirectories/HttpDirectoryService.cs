using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpDirectories
{
	public class HttpDirectoryService
	{

		public IHttpDirectory Directory = new HttpDirectory();
		public Dictionary<HttpStatus, IPage> ErrorPage = new Dictionary<HttpStatus, IPage>();
		public IErrorPage DefaultErrorPage = new DefaultErrorPage();
		public IPage GetErrorPage(HttpStatus status)
		{
			if (this.ErrorPage.ContainsKey(status))
				return this.ErrorPage[status];
			else
			{
				DefaultErrorPage.Status = status;
				return DefaultErrorPage;
			}
		}
		public IPage GetPage(HttpPath path)
		{

			while (path.Prev != null)
			{
				path = path.Prev;
			}
			HttpDirectoryObject obj;
			string requestPath;


			while (true)
			{
				requestPath = path.Path.Substring(1);
				if (Directory.Contains(requestPath) == false)
					return null;
				obj = Directory[requestPath];
				if (path == null)
					return null;
				if (obj is IHttpDirectory)
				{
					if (Directory.Contains(requestPath) == false)
						return null;

					obj = Directory[requestPath];
				}
				else if (obj is IPage && path.Next == null)
				{

					break;
				}
				path = path.Next;
			}
			return (IPage)obj;
		}
	}
}
