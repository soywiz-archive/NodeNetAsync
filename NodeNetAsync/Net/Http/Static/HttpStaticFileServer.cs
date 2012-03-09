using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.OS;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Net.Http.Static
{
	public class HttpStaticFileServer : IHttpFilter
	{
		public struct ResultStruct
		{
			public string RealFilePath;
			public string ContentType;
			public long Size;
			public byte[] Data;
		}

		protected string Path;
		AsyncCache<string, ResultStruct> Cache = new AsyncCache<string, ResultStruct>();
		public long CacheSizeThresold = 512 * 1024; // 0.5 MB

		public HttpStaticFileServer(string Path, bool Cache = true)
		{
			this.Path = Path;
			this.Cache.Enabled = Cache;
		}

		async Task IHttpFilter.Filter(HttpRequest Request, HttpResponse Response)
		{
			var FilePath = Url.GetInnerFileRelativeToPath(this.Path, Request.Url);

			var CachedResult = await Cache.GetAsync(FilePath, async () =>
			{
				await Console.Out.WriteLineAsync(String.Format("Caching '{0}'", FilePath));

				if (Directory.Exists(FilePath))
				{
					FilePath = FilePath + "/index.html";
				}

				var FileInfo = await FileSystem.GetFileInfoAsync(FilePath);
				var Size = FileInfo.Length;

				return new ResultStruct()
				{
					RealFilePath = FilePath,
					ContentType = MimeType.GetFromPath(FilePath),
					Size = Size,
					Data = (Size <= CacheSizeThresold) ? (await FileSystem.ReadAllBytesAsync(FilePath)) : null,
				};
			});

			Response.Buffering = true;

			Response.Headers["Content-Type"] = CachedResult.ContentType;
			Response.Headers["Content-Length"] = CachedResult.Size.ToString();
			Response.ChunkedTransferEncoding = false;

			// Cached byte[]
			if (CachedResult.Data != null)
			{
				await Response.WriteAsync(CachedResult.Data);
			}
			// No cached byte[], stream the file
			else
			{
				await Response.StreamFileASync(CachedResult.RealFilePath);
			}
		}
	}
}
