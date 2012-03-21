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
