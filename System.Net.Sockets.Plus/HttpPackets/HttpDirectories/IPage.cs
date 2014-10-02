using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpDirectories
{
	public abstract class IPage : HttpDirectoryObject
	{

		public string ContentType;
		public IPage()
		{
			this.Encode = new UTF8Encoding();
		}

		public virtual Encoding Encode { get; private set; }



		public string OutputBinary(byte[] data)
		{
			return Encode.GetString(data);
		}
		public string OutputFile(string path)
		{
			if (File.Exists(path) == false)
				throw new FileNotFoundException();

			byte[] data = File.ReadAllBytes(path);
			return OutputBinary(data);
		}




	}
}
