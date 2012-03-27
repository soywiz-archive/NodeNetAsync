using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Static;
using NodeNetAsync.Vfs;

namespace NodeNetAsync.Yui
{
	/// <summary>
	/// Adds support for automatical JS minimizing and serving combined JS files.
	/// Minification is done using YUI compressor with IKVM.
	/// 
	/// For .js files : It will minimize JS files before serving them.
	/// For .jsx files: It will read all the lines (that will be references to .js files) from the JSX and will combine and minimize them.
	/// 
	/// Trying to access a .jsx file will cause a 404 error.
	/// To reference a .jsx file you should reference to a file with the same name and path and the extension changed to .js
	/// </summary>
	public class NodeNetJsFilter : IHttpStaticFilter
	{
		bool Compressing;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Compressing"></param>
		public NodeNetJsFilter(bool Compressing = true)
		{
			this.Compressing = Compressing;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="HttpStaticFileServer"></param>
		void IHttpStaticFilter.RegisterFilters(HttpStaticFileServer HttpStaticFileServer)
		{
			HttpStaticFileServer.AddExtensionHandler("js", JsHandler);
			HttpStaticFileServer.AddExtensionHandler("jsx", JsxHandler);
		}

		/// <summary>
		/// Trying to access the JSX file directly will cause a 404 error to avoid accessing the list of files.
		/// </summary>
		/// <param name="Parameter"></param>
		/// <returns></returns>
		protected Task<HttpStaticFileServer.ResultStruct> JsxHandler(HttpStaticFileServer.HandlerStruct Parameter)
		{
			throw (new HttpException(HttpCode.NOT_FOUND_404));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Parameter"></param>
		/// <returns></returns>
		async protected Task<HttpStaticFileServer.ResultStruct> JsHandler(HttpStaticFileServer.HandlerStruct Parameter)
		{
			var FileSystem = Parameter.FileSystem;
			var JsFileName = Parameter.FilePath;
			var JsxFileName = Parameter.FilePath.FullPathWithoutExtension + ".jsx";
			var JsFileInfo = await Parameter.FileSystem.GetFileInfoAsync(JsFileName);
			var JsxFileInfo = await Parameter.FileSystem.GetFileInfoAsync(JsxFileName);
			DateTime CombinedJsDateTimeUtc = new DateTime();
			string CombinedJsFile = "";

			// A .jsx file exists
			if (JsxFileInfo.Exists)
			{
				Parameter.AddCacheRelatedFile(JsxFileName);

				var JsxFile = await FileSystem.ReadAllContentAsStringAsync(JsxFileName, Encoding.UTF8);
				foreach (var _Part in JsxFile.Split('\n', '\r'))
				{
					var JsComponentFileName = _Part.Trim();
					if (JsComponentFileName.Length > 0)
					{
						Parameter.AddCacheRelatedFile(JsComponentFileName);

						var JsComponentFileInfo = await FileSystem.GetFileInfoAsync(JsComponentFileName);

						CombinedJsFile += await FileSystem.ReadAllContentAsStringAsync(JsComponentFileName, Encoding.UTF8);
						if (CombinedJsDateTimeUtc < JsComponentFileInfo.LastWriteTimeUtc)
						{
							CombinedJsDateTimeUtc = JsComponentFileInfo.LastWriteTimeUtc;
						}
						//
						//Console.WriteLine(Part);
					}
				}
			}
			// Not existing a .jsx file, serve the .js file
			else
			{
				CombinedJsFile = await FileSystem.ReadAllContentAsStringAsync(JsFileName, Encoding.UTF8);
				CombinedJsDateTimeUtc = JsFileInfo.LastWriteTimeUtc;
			}

			if (this.Compressing)
			{
				CombinedJsFile = await Compressor.CompressJavaScriptAsync(CombinedJsFile);
			}

			var JsFileArray = Encoding.UTF8.GetBytes(CombinedJsFile);

			return new HttpStaticFileServer.ResultStruct()
			{
				ContentType = "text/javascript",
				Data = JsFileArray,
				RealFilePath = JsFileName,
				FileInfo = new VirtualFileInfo()
				{
					Length = JsFileArray.Length,
					LastWriteTimeUtc = CombinedJsDateTimeUtc,
				}
			};
		}
	}
}
