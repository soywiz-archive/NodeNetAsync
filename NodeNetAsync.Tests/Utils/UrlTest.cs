using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class UrlTest
	{
		[TestMethod]
		public void ExpandDirectoriesTest()
		{
			Assert.AreEqual("Path/To/File2", Url.Normalize("Path/To/File/.././//File2"));
			Assert.AreEqual("/Path/To/File2", Url.Normalize("/Path/To/File2"));
			Assert.AreEqual("", Url.Normalize(""));
			Assert.AreEqual("/", Url.Normalize("/"));
			Assert.AreEqual("/", Url.Normalize("////"));
			Assert.AreEqual("/", Url.Normalize(@"\\\\"));
			Assert.AreEqual("/", Url.Normalize("/../.././"));
		}

		[TestMethod]
		public void GetInnerFileRelativeToPathTest()
		{
			Assert.AreEqual("C:/path/to/file/file.txt", Url.GetInnerFileRelativeToPath(@"C:\path\to\file", "../../file.txt"));
			Assert.AreEqual("C:/path/to/file/file.txt", Url.GetInnerFileRelativeToPath(@"C:\path\to\file", "/file.txt"));
			Assert.AreEqual("C:/path/to/file/file.txt", Url.GetInnerFileRelativeToPath(@"C:\path\to\file/", "/file.txt"));
		}
	}
}
