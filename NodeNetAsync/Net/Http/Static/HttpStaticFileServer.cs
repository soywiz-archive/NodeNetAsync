using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
			public FileInfo FileInfo;
			public byte[] Data;
			public string ETag;
		}

		protected string StaticPath;
		AsyncCache<string, ResultStruct> Cache = new AsyncCache<string, ResultStruct>(Enabled: true);
		public long CacheSizeThresold = 512 * 1024; // 0.5 MB
		protected FileSystemWatcher FileSystemWatcher;

		public HttpStaticFileServer(string Path, bool Cache = true)
		{
			this.StaticPath = Path;
			this.Cache.Enabled = Cache;
			this.FileSystemWatcher = new FileSystemWatcher(Path);
			FileSystemWatcher.IncludeSubdirectories = true;
			FileSystemWatcher.Created += FileSystemWatcher_Updated;
			FileSystemWatcher.Changed += FileSystemWatcher_Updated;
			FileSystemWatcher.Deleted += FileSystemWatcher_Updated;
			FileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
			FileSystemWatcher.EnableRaisingEvents = true;
		}

		void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			_FileSystemWatcher_Updated(e.OldName);
			_FileSystemWatcher_Updated(e.Name);
		}

		void FileSystemWatcher_Updated(object sender, FileSystemEventArgs e)
		{
			_FileSystemWatcher_Updated(e.Name);
		}

		void _FileSystemWatcher_Updated(string Name)
		{
			var FilePath = Url.GetInnerFileRelativeToPath(this.StaticPath, Name);
			//var CacheKey = FilePath.ToLowerInvariant();
			var CacheKey = FilePath;
			Cache.Remove(CacheKey);

			if (Cache.Enabled)
			{
				Task.Run(async () =>
				{
					await Console.Out.WriteLineAsync(String.Format("Flushed '{0}'", CacheKey));
				});
			}
		}

		async Task IHttpFilter.FilterAsync(HttpRequest Request, HttpResponse Response)
		{
			var Uri = Request.Url;
			Uri = Uri.Split(new[] { '?' }, 2)[0];
			var FilePath = Url.GetInnerFileRelativeToPath(this.StaticPath, Uri);
			if (Path.GetExtension(FilePath) == "")
			{
				if (Directory.Exists(FilePath))
				{
					FilePath = Url.ExpandDirectories(FilePath + "/index.html");
				}
			}

			//var CacheKey = FilePath.ToLowerInvariant();
			var CacheKey = FilePath;
			var CachedResult = await Cache.GetAsync(CacheKey, async () =>
			{
				if (Cache.Enabled)
				{
					await Console.Out.WriteLineAsync(String.Format("Caching '{0}'", CacheKey));
				}

				var FileInfo = await FileSystem.GetFileInfoAsync(FilePath);
				if (FileInfo.Exists)
				{
					bool ShouldCacheInMemory = FileInfo.Length <= CacheSizeThresold;
					string ETag = null;
					byte[] Data = null;

					if (ShouldCacheInMemory)
					{
						Data = await FileSystem.ReadAllBytesAsync(FilePath);
						ETag = BitConverter.ToString(MD5.Create().ComputeHash(Data));
					}

					return new ResultStruct()
					{
						RealFilePath = FilePath,
						ContentType = MimeType.GetFromPath(FilePath),
						FileInfo = FileInfo,
						ETag = ETag,
						Data = Data,
					};
				}
				else
				{
					throw(new HttpException(HttpCode.NOT_FOUND_404));
				}
			});

			// Check ETag
			if (Request.Headers["If-None-Match"] != "")
			{
				if (Request.Headers["If-None-Match"] == CachedResult.ETag)
				{
					throw(new HttpException(HttpCode.NOT_MODIFIED_304));
				}
			}

			// Check Last-Modified
			if (Request.Headers["If-Modified-Since"] != "")
			{
				var RequestIfModifiedSince = DateTime.ParseExact(Request.Headers["If-Modified-Since"], "R", CultureInfo.InvariantCulture);

				if (RequestIfModifiedSince == CachedResult.FileInfo.LastWriteTimeUtc)
				{
					throw(new HttpException(HttpCode.NOT_MODIFIED_304));
				}
			}

			Response.Buffering = true;
			Response.ChunkedTransferEncoding = false;

			/*
			Cache-Control:max-age=2419200, private
			Connection:keep-alive
			Date:Wed, 14 Mar 2012 14:52:54 GMT
			Expires:Wed, 11 Apr 2012 14:52:54 GMT
			Last-Modified:Wed, 11 Jan 2012 13:52:46 GMT
			*/

			Response.Headers["Content-Type"] = CachedResult.ContentType;
			Response.Headers["Content-Length"] = CachedResult.FileInfo.Length.ToString();
			if (CachedResult.ETag != null)
			{
				Response.Headers["ETag"] = CachedResult.ETag;
			}
			Response.Headers["Date"] = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
			Response.Headers["Last-Modified"] = CachedResult.FileInfo.LastWriteTimeUtc.ToString("R", CultureInfo.InvariantCulture);

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
