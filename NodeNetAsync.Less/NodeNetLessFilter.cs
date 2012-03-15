using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotless.Core;
using dotless.Core.configuration;
using NodeNetAsync.Net.Http.Static;
using NodeNetAsync.OS;
using NodeNetAsync.Vfs;

namespace NodeNetAsync.Less
{
	public class NodeNetLessFilter
	{
		protected static DotlessConfiguration Config;
		protected static ILessEngine Engine;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="LessCode"></param>
		/// <returns></returns>
		async static public Task<string> TransformAsync(string LessCode, string FileName = "unknown.less")
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
		async static public Task<HttpStaticFileServer.ResultStruct> StaticHandler(HttpStaticFileServer.HandlerStruct Parameter)
		{
			var FileSystem = Parameter.FileSystem;

			var LessFileName = Parameter.FilePath.Substring(0, Parameter.FilePath.Length - 4) + ".less";
			var LessFileInfo = await FileSystem.GetFileInfoAsync(LessFileName);
			var CssFile = await TransformAsync(await FileSystem.ReadAllContentAsStringAsync(LessFileName, Encoding.UTF8), LessFileName);

			Parameter.AddCacheRelatedFile(LessFileName);

			var CssByteArray = Encoding.UTF8.GetBytes(CssFile);
			return new HttpStaticFileServer.ResultStruct()
			{
				ContentType = "text/css",
				Data = CssByteArray,
				RealFilePath = LessFileName,
				FileInfo = new VirtualFileInfo()
				{
					Length = CssByteArray.Length,
					LastWriteTimeUtc = LessFileInfo.LastWriteTimeUtc,
				}
			};
		}
	}
}
