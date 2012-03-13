using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class AsyncTaskQueueTest
	{
		[TestMethod]
		async public Task TestEnqueueAsync()
		{
			var Results = new List<int>();
			var AsyncTaskQueue = new AsyncTaskQueue();
			var Task1 = AsyncTaskQueue.EnqueueAsync(async () => { Results.Add(1); await Task.Delay(10); Results.Add(2); });
			var Task2 = AsyncTaskQueue.EnqueueAsync(async () => { Results.Add(3); await Task.Delay(5); Results.Add(4); });
			var Task3 = AsyncTaskQueue.EnqueueAsync(async () => { Results.Add(5); await Task.Delay(0); Results.Add(6); });
			await AsyncTaskQueue.WaitAllAsync();
			//await Task.WhenAll(Task1, Task2, Task3);
			Assert.AreEqual("1,2,3,4,5,6", String.Join(",", Results));
		}
	}
}
