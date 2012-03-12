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
		public void TestSimpleClass()
		{
			Assert.AreEqual("{\"Array\":[1,2,3,4],\"Integer\":5}", JsonSerializer.Serialize(new TestClass()).ToString());
		}

		[TestMethod]
		public void TestBoolean()
		{
			Assert.AreEqual("true", JsonSerializer.Serialize(true).ToString());
		}

		[TestMethod]
		public void TestInteger()
		{
			Assert.AreEqual("1", JsonSerializer.Serialize(1).ToString());
		}

		[TestMethod]
		public void TestString()
		{
			Assert.AreEqual("\"test\"", JsonSerializer.Serialize("test").ToString());
		}

		[TestMethod]
		public void TestIntegerArray()
		{
			Assert.AreEqual("[1,2,3]", JsonSerializer.Serialize(new int[] { 1, 2, 3 }).ToString());
		}

		[TestMethod]
		public void TestMixedArray()
		{
			Assert.AreEqual("[1,true,3]", JsonSerializer.Serialize(new object[] { 1, true, 3 }).ToString());
		}

		[TestMethod]
		public void TestAnonymousObject()
		{
			Assert.AreEqual(@"{""hello"":""1"",""world"":{""test"":2}}", JsonSerializer.Serialize(new { hello = "1", world = new { test = 2 } }).ToString());
		}
	}
}
