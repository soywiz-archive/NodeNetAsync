using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class ProducerConsumerTest
	{
		[TestMethod]
		public void TestProducerConsumer()
		{
			var ProducerConsumer = new ProducerConsumer<byte>();
			Assert.AreEqual(0, ProducerConsumer.AvailableForRead);
			ProducerConsumer.Write(Encoding.UTF8.GetBytes("Hello World!"));
			Assert.AreEqual(12, ProducerConsumer.AvailableForRead);
			ProducerConsumer.Skip(0);
			Assert.AreEqual(12, ProducerConsumer.AvailableForRead);
			ProducerConsumer.Skip(6);
			Assert.AreEqual(6, ProducerConsumer.AvailableForRead);
			Assert.AreEqual("World!", Encoding.UTF8.GetString(ProducerConsumer.PeekGet(6)));
			Assert.AreEqual(6, ProducerConsumer.AvailableForRead);
		}
	}
}
