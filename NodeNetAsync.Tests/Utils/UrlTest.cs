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
			Assert.AreEqual("Path/To/File2", Url.ExpandDirectories("Path/To/File/.././//File2"));
			Assert.AreEqual("/Path/To/File2", Url.ExpandDirectories("/Path/To/File2"));
			Assert.AreEqual("", Url.ExpandDirectories(""));
			Assert.AreEqual("/", Url.ExpandDirectories("/"));
			Assert.AreEqual("/", Url.ExpandDirectories("////"));
			Assert.AreEqual("/", Url.ExpandDirectories(@"\\\\"));
			Assert.AreEqual("/", Url.ExpandDirectories("/../.././"));
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
