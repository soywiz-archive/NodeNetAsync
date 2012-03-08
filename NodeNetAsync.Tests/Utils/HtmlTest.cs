using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Tests.Utils
{
	[TestClass]
	public class HtmlTest
	{
		[TestMethod]
		public void QuoteTest()
		{
			Assert.AreEqual("&lt;test&gt;", Html.Quote("<test>"));
		}
	}
}
