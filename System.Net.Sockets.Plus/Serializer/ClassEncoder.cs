using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.Serializer
{
	public class ClassEncoder<T, TClass> : IPacketEncoder<T, TClass, TClass>
	{


		#region IPacketEncoder<T,TClass,TClass> メンバー

		public byte[] Encode(TClass packet, SocketClient<T, TClass, TClass> client)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
