using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotless.Core;
using dotless.Core.configuration;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Static;
using NodeNetAsync.OS;
using NodeNetAsync.Vfs;
using NodeNetAsync.Yui;

namespace NodeNetAsync.Less
{
	/// <summary>
	/// Adds Css less support and css minimification.
	/// Less support is done using dotless.
	/// Minification is perform using YUI compressor with IKVM.
	/// 
	/// For .less files: It will transform the .less file into a .css one and will minimize it.
	/// For .css files : It will minimize it.
	/// 
	/// Trying to access a .less file directly will cause a 404 error.
	/// To reference a .less file you should reference to a file with the same name and path and the extension changed to .css
	/// </summary>
	public class NodeNetLessFilter : IHttpStaticFilter
	{
		protected static DotlessConfiguration Config;
		protected static ILessEngine Engine;
		protected bool Compressing;

		public NodeNetLessFilter(bool Compressing = true)
		{
			this.Compressing = Compressing;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="HttpStaticFileServer"></param>
		void IHttpStaticFilter.RegisterFilters(HttpStaticFileServer HttpStaticFileServer)
		{
			HttpStaticFileServer.AddExtensionHandler("css", CssHandler);
			HttpStaticFileServer.AddExtensionHandler("less", LessHandler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Parameter"></param>
		/// <returns></returns>
		protected Task<HttpStaticFileServer.ResultStruct> LessHandler(HttpStaticFileServer.HandlerStruct Parameter)
		{
			throw (new HttpException(HttpCode.NOT_FOUND_404));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="LessCode"></param>
		/// <returns></returns>
		async static protected Task<string> TransformAsync(string LessCode, string FileName = "unknown.less")
		{
			return await Task.Run(() =>
			{
				if (Config == null)
				{
					Config = new DotlessConfiguration()
					{
						MinifyOutput = true,
					};
				}
				if (Engine == null)
				{
					Engine = new EngineFactory(Config).GetEngine();
				}
				return Engine.TransformToCss(LessCode, FileName);
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="CssFileName"></param>
		/// <returns></returns>
		async protected Task<HttpStaticFileServer.ResultStruct> CssHandler(HttpStaticFileServer.HandlerStruct Parameter)
		{
			var FileSystem = Parameter.FileSystem;

			var CssFileName = Parameter.FilePath;

			var CssFileInfo = await FileSystem.GetFileInfoAsync(CssFileName);
			string RealFilePath;
			string CssFile;
			byte[] CssByteArray;
			DateTime LastWriteTimeUtc;

			if (CssFileInfo.Exists)
			{
				RealFilePath = CssFileName;
				CssFile = await FileSystem.ReadAllContentAsStringAsync(CssFileName, Encoding.UTF8);
				CssByteArray = Encoding.UTF8.GetBytes(CssFile);
				LastWriteTimeUtc = CssFileInfo.LastWriteTimeUtc;
			}
			else
			{
				var LessFileName = CssFileName.FullPathWithoutExtension + ".less";
				var LessFileInfo = await FileSystem.GetFileInfoAsync(LessFileName);
				RealFilePath = LessFileName;
				CssFile = await TransformAsync(await FileSystem.ReadAllContentAsStringAsync(LessFileName, Encoding.UTF8), LessFileName);
				Parameter.AddCacheRelatedFile(LessFileName);
				LastWriteTimeUtc = LessFileInfo.LastWriteTimeUtc;
			}

			if (Compressing)
			{
				CssFile = await Compressor.CompressCssAsync(CssFile);
			}

			CssByteArray = Encoding.UTF8.GetBytes(CssFile);

			return new HttpStaticFileServer.ResultStruct()
			{
				ContentType = "text/css",
				Data = CssByteArray,
				RealFilePath = RealFilePath,
				FileInfo = new VirtualFileInfo()
				{
					Length = CssByteArray.Length,
					LastWriteTimeUtc = LastWriteTimeUtc,
				}
			};
		}
	}
}
