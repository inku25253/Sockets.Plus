using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.Serializer
{
	public class ClassEncoder<T, TClass> : IPacketEncoder<T, TClass>
	{
		#region IPacketEncoder<T,TClass> メンバー

		public byte[] Encode(TClass obj, SocketClient<T, TClass> client)
		{
			return new byte[0];
		}

		#endregion
	}
}
