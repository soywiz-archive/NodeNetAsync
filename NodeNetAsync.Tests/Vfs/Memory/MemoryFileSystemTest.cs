using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Vfs.Memory;

namespace NodeNetAsync.Tests.Vfs.Memory
{
	[TestClass]
	public class MemoryFileSystemTest
	{
		MemoryFileSystem FileSystem = new MemoryFileSystem();

		[TestMethod]
		async public Task TestMemoryFileSystem()
		{
			await FileSystem.WriteAllContentAsync("/file.txt", "Hello World!", Encoding.UTF8);
			Assert.AreEqual("Hello World!", await FileSystem.ReadAllContentAsStringAsync("/file.txt", Encoding.UTF8));
		}
	}
}
