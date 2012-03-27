using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.yahoo.platform.yui.compressor;
using NodeNetAsync.Utils;
using org.mozilla.javascript;

namespace NodeNetAsync.Yui
{
	public class Compressor
	{
		async static public Task<string> CompressCssAsync(string CssString)
		{
			return await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				var CssWriter = new StringWriter();
				var CssCompressor = new CssCompressor(new StringReader(CssString).GetJavaReader());
				CssCompressor.compress(CssWriter.GetJavaWriter(), 1024);

				return CssWriter.ToString();
			});
		}

		async static public Task<string> CompressJavaScriptAsync(string JavaScriptString)
		{
			return await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				var JsWriter = new StringWriter();
				var javaScriptCompressor = new JavaScriptCompressor(new StringReader(JavaScriptString).GetJavaReader(), new ErrorReporterMock());
				javaScriptCompressor.compress(JsWriter.GetJavaWriter(), 1024, true, false, false, false);
				return JsWriter.ToString();
			});
		}

		internal class ErrorReporterMock : ErrorReporter
		{
			public void error(string str1, string str2, int i1, string str3, int i2)
			{
				throw new NotImplementedException();
			}

			public EvaluatorException runtimeError(string str1, string str2, int i1, string str3, int i2)
			{
				throw new NotImplementedException();
			}

			public void warning(string str1, string str2, int i1, string str3, int i2)
			{
				throw new NotImplementedException();
			}
		}
	}
}
