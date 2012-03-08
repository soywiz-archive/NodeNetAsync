using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Json;

namespace NodeNetAsync.Tests.Net.Http
{
	[TestClass]
	public class HttpHeadersTest
	{
		[TestMethod]
		public void HttpHeadersAddTest()
		{
			var HttpHeaders = new HttpHeaders();
			Assert.AreEqual("[]", HttpHeaders.AsEnumerable<HttpHeader>().ToJsonString());
			HttpHeaders.Add("Content-Type", "text/html");
			HttpHeaders.Set("Content-Type", "text/plain");
			HttpHeaders.Set("cOntEnt-type", "text/plain");
			Assert.AreEqual(1, HttpHeaders.Count());
			Assert.AreEqual("cOntEnt-type: text/plain", String.Join("|", HttpHeaders.AsEnumerable()));
		}
	}
}
