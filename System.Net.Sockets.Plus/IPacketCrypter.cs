using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public interface IPacketCrypter
	{
		byte[] Encrypt(byte[] packet, int offset, int count);

		byte[] Decrypt(byte[] cPacket, int offset, int count);
	}
}
