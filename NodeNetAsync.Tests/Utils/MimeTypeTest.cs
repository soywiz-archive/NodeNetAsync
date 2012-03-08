using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class MimeTypeTest
	{
		[TestMethod]
		public void GetFromPathTest()
		{
			Assert.AreEqual("text/html", MimeType.GetFromPath("C:/Path/To/File"));
			Assert.AreEqual("text/html", MimeType.GetFromPath("C:/Path/To/File.html"));
			Assert.AreEqual("text/plain", MimeType.GetFromPath("C:/Path/To/File.txt"));
			Assert.AreEqual("octet/stream", MimeType.GetFromPath("C:/Path/To/File.bin"));
		}
	}
}
