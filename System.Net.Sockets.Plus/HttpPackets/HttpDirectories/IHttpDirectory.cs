using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus.HttpPackets.HttpDirectories
{
	public abstract class IHttpDirectory : HttpDirectoryObject
	{
		Dictionary<string, HttpDirectoryObject> directoryObjects = new Dictionary<string, HttpDirectoryObject>();
		public HttpDirectoryObject this[string index]
		{
			get { return directoryObjects[index]; }
			set { directoryObjects[index] = value; }
		}
		public bool Contains(string path)
		{
			return this.directoryObjects.ContainsKey(path);
		}
		/*
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (directoryObjects.ContainsKey(binder.Name))
			{

				this[binder.Name] = (HttpDirectoryObject)value;


				return true;
			}
			return false;
		}
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (directoryObjects.ContainsKey(binder.Name))
			{

				result = directoryObjects[binder.Name];
				return true;
			}
			result = null;
			return false;
		}*/


	}
}
