using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class ByteRingBufferTest
	{
		[TestMethod]
		public void TestByteRingBuffer()
		{
			var CurrentEncoding = Encoding.UTF8;
			int BufferSize = 1024;
			var Buffer = new RingBuffer<byte>(BufferSize);
			
			// Initial check
			Assert.AreEqual(BufferSize, Buffer.AvailableForWrite);
			Assert.AreEqual(0, Buffer.AvailableForRead);
			
			// Try peek
			CollectionAssert.AreEqual(new byte[0], Buffer.PeekGet(16));
	
			// Write
			var DataToWrite = CurrentEncoding.GetBytes("Hello World");
			Buffer.Write(DataToWrite);

			Assert.AreEqual(BufferSize - DataToWrite.Length, Buffer.AvailableForWrite);
			Assert.AreEqual(0 + DataToWrite.Length, Buffer.AvailableForRead);
		}
	}
}
