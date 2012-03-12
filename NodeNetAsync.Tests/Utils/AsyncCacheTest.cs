using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.OS;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class AsyncCacheTest
	{
		AsyncCache<string, string> Cache = new AsyncCache<string, string>();
		protected int ExecutedGetterCount = 0;

		async private Task<string> Test(string Key)
		{
			return await Cache.GetAsync(Key, async () =>
			{
				ExecutedGetterCount++;
				await Task.Delay(1);
				return Key + "1";
			});
		}

		[TestMethod]
		async public Task TestGetAsyncExecutedOnce()
		{
			ExecutedGetterCount = 0;
			var Task1 = Test("test");
			var Task2 = Test("test");
			await Task.WhenAll(Task1, Task2);

			Assert.AreEqual(1, ExecutedGetterCount);
			Assert.AreEqual("test1", await Task1);
			Assert.AreEqual("test1", await Task2);
		}
	}
}
