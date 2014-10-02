using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	/// <summary>
	/// SimpleActivatorで引用にSocketClientを渡したいときに使用。
	/// <example>
	/// Server.Activator = new SimpleActivator(typeof(SocketClientRequest), Arguments.....);
	/// </example>
	/// </summary>
	public sealed class SocketClientRequest
	{
	}
}
