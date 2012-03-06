using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Json;

namespace NodeNetAsync.Tests.Json
{
	public class TestClass
	{
		public int[] Array = new int[] { 1, 2, 3, 4 };
		public int Integer = 5;
	}

	[TestClass]
	public class JsonSerializerTest
	{
		[TestMethod]
		public void TestJsonSerializing()
		{
			Assert.AreEqual("true", JsonSerializer.Serialize(true).ToString());
			Assert.AreEqual("1", JsonSerializer.Serialize(1).ToString());
			Assert.AreEqual("\"test\"", JsonSerializer.Serialize("test").ToString());
			Assert.AreEqual("[1,2,3]", JsonSerializer.Serialize(new int[] { 1, 2, 3 }).ToString());
			Assert.AreEqual("[1,true,3]", JsonSerializer.Serialize(new object[] { 1, true, 3 }).ToString());
			Assert.AreEqual("{\"Array\":[1,2,3,4],\"Integer\":5}", JsonSerializer.Serialize(new TestClass()).ToString());
		}
	}
}
