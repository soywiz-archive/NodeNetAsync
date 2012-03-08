using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Db.Redis;
using NodeNetAsync.Net;

namespace NodeNetAsync.Tests.Db.Redis
{
	[TestClass]
	public class RedisClientTest
	{
		[TestMethod]
		async public Task TestConnection()
		{
			await TestTcpServer.Create(
				ServerClientHandle: async (Client) =>
				{
					// SET
					Assert.AreEqual("*3", await Client.ReadLineAsync());
					Assert.AreEqual("$3", await Client.ReadLineAsync());
					Assert.AreEqual("set\r\n", await Client.ReadStringAsync(3 + 2));
					Assert.AreEqual("$3", await Client.ReadLineAsync());
					Assert.AreEqual("foo\r\n", await Client.ReadStringAsync(3 + 2));
					Assert.AreEqual("$4", await Client.ReadLineAsync());
					Assert.AreEqual("test\r\n", await Client.ReadStringAsync(4 + 2));
					await Client.WriteAsync("+OK\r\n");

					// GET
					Assert.AreEqual("*2", await Client.ReadLineAsync());
					Assert.AreEqual("$3", await Client.ReadLineAsync());
					Assert.AreEqual("get\r\n", await Client.ReadStringAsync(3 + 2));
					Assert.AreEqual("$3", await Client.ReadLineAsync());
					Assert.AreEqual("foo\r\n", await Client.ReadStringAsync(3 + 2));
					
					await Client.WriteAsync("$4\r\ntest\r\n");
				},
				ClientConnection: async (TestPort) =>
				{
					var Redis = await RedisClient.CreateAndConnectAsync(Host: "127.0.0.1", Port: TestPort);
					await Redis.SetAsync("foo", "test");
					Assert.AreEqual("test", await Redis.GetAsync("foo"));
				},
				OnError: (Exception) =>
				{
					Assert.Fail();
				}
			);
		}
	}
}
