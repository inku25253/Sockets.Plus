using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets.Plus;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{

		[TestMethod]
		public void ポート使用可能テスト()
		{
			Assert.IsTrue(SocketServer.CheckPort(980));
		}

		[TestMethod]
		public void ポート使用不可テスト()
		{

			SocketServer server = new SocketServer();
			server.Setup(980);
			server.Start();

			Assert.IsFalse(SocketServer.CheckPort(980));


		}
	}
}
