using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Streams;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Streams
{
	[TestClass]
	public class NodeBufferedStreamTest
	{
		[TestMethod]
		async public Task TestReadLineAsync()
		{
			var MemoryStream = new MemoryStream();
			var StreamWriter = new StreamWriter(MemoryStream) { AutoFlush = true };
			StreamWriter.Write("a\r\n");
			StreamWriter.Write("\r\n");
			StreamWriter.Write("bb\r\n");
			StreamWriter.Write("ccc\r\n");
			StreamWriter.Write("dddd\r\n");

			MemoryStream.Position = 0;
			var Stream = new NodeBufferedStream(MemoryStream, 3);

			Assert.AreEqual("a", await Stream.ReadLineAsync(Encoding.UTF8));
			Assert.AreEqual("", await Stream.ReadLineAsync(Encoding.UTF8));
			Assert.AreEqual("bb", await Stream.ReadLineAsync(Encoding.UTF8));
			Assert.AreEqual("ccc", await Stream.ReadLineAsync(Encoding.UTF8));
			Assert.AreEqual("dddd", await Stream.ReadLineAsync(Encoding.UTF8));
		}

		[TestMethod]
		async public Task ReadLineAsyncLatencyIssuesTest()
		{
			ThreadPool.SetMaxThreads(1, 1);
			var MemoryStream = new ProducerConsumerStream(new ProducerConsumer<byte>());
			var StreamWriter = new StreamWriter(MemoryStream) { AutoFlush = true };
			StreamWriter.Write("line1\r");

			Task.Run(async () =>
			{
				await Task.Delay(2);
				StreamWriter.Write("\n");
				StreamWriter.Write("line2\r\n");
			});

			var Stream = new NodeBufferedStream(MemoryStream, 3);

			Assert.AreEqual("line1", await Stream.ReadLineAsync(Encoding.UTF8));
			Assert.AreEqual("line2", await Stream.ReadLineAsync(Encoding.UTF8));
		}
	}
}
