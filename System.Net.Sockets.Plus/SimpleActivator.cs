using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Sockets.Plus
{
	public class SimpleActivator<TState, TSendPacket, TReceivePacket> : IActivator<TState, TSendPacket, TReceivePacket>
	{
		public object[] Arguments;

		public SimpleActivator(params object[] args)
		{
			this.Arguments = args;
		}

		public void SetArguments(params object[] args)
		{
			this.Arguments = args;
		}




		#region IActivator<TState,TPacket> メンバー

		TState IActivator<TState, TSendPacket, TReceivePacket>.Activate(SocketEventArgs<TState, TSendPacket, TReceivePacket> args)
		{
			object[] instanceArgument = new object[Arguments.Length];
			for (int i = 0; i < Arguments.Length; ++i)
			{
				object current = Arguments[i];
				if (current == typeof(SocketClientRequest))
				{
					current = args.Client;
				}
				instanceArgument[i] = current;
			}
			Type[] argumentTypes = new Type[instanceArgument.Length];


			for (int i = 0; i < argumentTypes.Length; ++i)
			{
				argumentTypes[i] = instanceArgument[i].GetType();
			}


			var ctor = typeof(TState).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, argumentTypes, null);



			return (TState)ctor.Invoke(instanceArgument);//Activator.CreateInstance(typeof(TState), instanceArgument);
		}

		#endregion

	}
}
