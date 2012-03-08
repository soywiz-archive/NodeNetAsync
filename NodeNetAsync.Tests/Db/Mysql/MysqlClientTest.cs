using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Db.Mysql;
using NodeNetAsync.Net;

namespace NodeNetAsync.Tests.Db.Mysql
{
	[TestClass]
	public class MysqlClientTest
	{
		[TestMethod]
		async public Task TestMethod2()
		{
			await TestTcpServer.Create(
				Server: async (Client) =>
				{
				},
				Client: async (TestPort) =>
				{
					var Mysql = new MysqlClient(Host: "127.0.0.1", Port: TestPort);
					await Mysql.QueryAsync("SELECT 1;");
				}
			);
		}
	}
}
