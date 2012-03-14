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
		protected FileSystemWatcher FileSystemWatcher;

		public HttpStaticFileServer(string Path, bool Cache = true)
		{
			this.Path = Path;
			this.Cache.Enabled = Cache;
			this.FileSystemWatcher = new FileSystemWatcher(Path);
			FileSystemWatcher.IncludeSubdirectories = true;
			FileSystemWatcher.Created += FileSystemWatcher_Updated;
			FileSystemWatcher.Changed += FileSystemWatcher_Updated;
			FileSystemWatcher.Deleted += FileSystemWatcher_Updated;
			FileSystemWatcher.EnableRaisingEvents = true;
		}

		void FileSystemWatcher_Updated(object sender, FileSystemEventArgs e)
		{
			var FilePath = Url.GetInnerFileRelativeToPath(this.Path, e.Name);
			Cache.Remove(FilePath);
		}

		async Task IHttpFilter.FilterAsync(HttpRequest Request, HttpResponse Response)
		{
			var Uri = Request.Url;
			Uri = Uri.Split(new[] { '?' }, 2)[0];
			var FilePath = Url.GetInnerFileRelativeToPath(this.Path, Uri);

			var CachedResult = await Cache.GetAsync(FilePath, async () =>
			{
				if (Cache.Enabled)
				{
					await Console.Out.WriteLineAsync(String.Format("Caching '{0}'", FilePath));
				}

				if (Directory.Exists(FilePath))
				{
					FilePath = FilePath + "/index.html";
				}

				var FileInfo = await FileSystem.GetFileInfoAsync(FilePath);
				if (FileInfo.Exists)
				{
					var Size = FileInfo.Length;

					return new ResultStruct()
					{
						RealFilePath = FilePath,
						ContentType = MimeType.GetFromPath(FilePath),
						Size = Size,
						Data = (Size <= CacheSizeThresold) ? (await FileSystem.ReadAllBytesAsync(FilePath)) : null,
					};
				}
				else
				{
					throw(new HttpException(HttpCode.NOT_FOUND_404));
				}
			});

			Response.Buffering = true;
			Response.ChunkedTransferEncoding = false;
			Response.Headers["Content-Type"] = CachedResult.ContentType;
			Response.Headers["Content-Length"] = CachedResult.Size.ToString();

			// Cached byte[]
			if (CachedResult.Data != null)
			{
				await Response.WriteAsync(CachedResult.Data);
			}
			// No cached byte[], stream the file
			else
			{
				Response.Buffering = false;
				//Response.ChunkedTransferEncoding = true;
				await Response.StreamFileASync(CachedResult.RealFilePath);
			}
		}
	}
}
