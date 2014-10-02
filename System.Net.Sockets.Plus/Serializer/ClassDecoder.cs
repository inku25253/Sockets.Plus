using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.Serializer
{
	public class ClassDecoder<T, TClass> : IPacketDecoder<T, TClass, TClass>
	{

		#region IPacketDecoder<T,TClass,TClass> メンバー

		public TClass Decode(object sender, SocketClient<T, TClass, TClass> client, SocketStream<T, TClass, TClass> stream)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
