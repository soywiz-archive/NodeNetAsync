using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class TaskExTest
	{
		class TestException : Exception { }

		[TestMethod]
		[ExpectedException(typeof(TestException))]
		async public Task TestRunPropagatingExceptionsAsync1()
		{
			await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				throw(new TestException());
			});
			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(TestException))]
		async public Task TestRunPropagatingExceptionsAsync2()
		{
			bool Value = true;
			var Return = await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				if (Value) throw (new TestException());
				return 1;
			});
			Assert.Fail();
		}

		[TestMethod]
		async public Task TestRunPropagatingExceptionsAsync3()
		{
			var Return = await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				return 1;
			});
			Assert.AreEqual(Return, 1);
		}
	}
}
