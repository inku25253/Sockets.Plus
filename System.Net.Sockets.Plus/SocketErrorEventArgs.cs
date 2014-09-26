using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public class SocketErrorEventArgs<T, TP> : SocketEventArgs<T, TP>
	{

		public Exception Exception { get; set; }
		public SocketErrorType Type { get; private set; }
		public SocketErrorEventArgs(Exception ex, SocketEventArgs<T, TP> socket, SocketErrorType type)
			: base(socket)
		{
			this.Exception = ex;
			this.Type = type;
		}
	}
	public enum SocketErrorType
	{
		Connect,
		Receive,
		Send,
		Unknown
	}
}
