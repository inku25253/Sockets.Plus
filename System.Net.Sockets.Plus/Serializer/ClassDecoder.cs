using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.Serializer
{
	public class ClassDecoder<T, TClass> : IPacketDecoder<T, TClass>
	{
		#region IPacketDecoder<T,TClass> メンバー

		public TClass Decode(object sender, SocketClient<T, TClass> client, SocketStream<T, TClass> net)
		{
			return default(TClass);
		}

		#endregion
	}
}
