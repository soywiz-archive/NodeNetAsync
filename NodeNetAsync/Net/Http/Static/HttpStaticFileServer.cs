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
using NodeNetAsync.Vfs;

namespace NodeNetAsync.Net.Http.Static
{
	public delegate Task<HttpStaticFileServer.ResultStruct> ExtensionHandlerDelegateAsync(HttpStaticFileServer.HandlerStruct HandlerStruct);

	public class HttpStaticFileServer : IHttpFilter
	{
		public struct ResultStruct
		{
			public string RealFilePath;
			public string ContentType;
			public VirtualFileInfo FileInfo;
			public bool Exists;
			public byte[] Data;
			public string ETag;
		}

		public class HandlerStruct
		{
			/// <summary>
			/// 
			/// </summary>
			public IVirtualFileSystem FileSystem;

			/// <summary>
			/// 
			/// </summary>
			public VirtualFilePath FilePath;

			/// <summary>
			/// 
			/// </summary>
			protected HttpStaticFileServer HttpStaticFileServer;

			internal HandlerStruct(HttpStaticFileServer HttpStaticFileServer)
			{
				this.HttpStaticFileServer = HttpStaticFileServer;
			}

			public void AddCacheRelatedFile(string File)
			{
				this.HttpStaticFileServer.AddUncacheChain(File, FilePath);
				Console.WriteLine("'{0}' -> '{1}'", File, FilePath);
			}
		}

		public long CacheSizeThresold = 512 * 1024; // 0.5 MB
		public IVirtualFileSystem VirtualFileSystem { get; protected set; }
		protected AsyncCache<string, ResultStruct> Cache = new AsyncCache<string, ResultStruct>(Enabled: true);

		protected Dictionary<string, SortedSet<string>> UncacheChain = new Dictionary<string, SortedSet<string>>();

		protected void AddUncacheChain(string File, string File2)
		{
			if (!UncacheChain.ContainsKey(File)) UncacheChain[File] = new SortedSet<string>();
			UncacheChain[File].Add(File2);
		}

		/// <summary>
		/// 
		/// </summary>
		Dictionary<string, ExtensionHandlerDelegateAsync> ExtensionHandlersAsync = new Dictionary<string, ExtensionHandlerDelegateAsync>();

		public HttpStaticFileServer(IVirtualFileSystem VirtualFileSystem, bool Cache = true)
		{
			this.VirtualFileSystem = VirtualFileSystem;
			this.Cache.Enabled = Cache;
			this.VirtualFileSystem.OnEvent += VirtualFileSystem_OnEvent;
		}

		void VirtualFileSystem_OnEvent(VirtualFileEvent obj)
		{
			if (obj.File1 != null) _FileSystemWatcher_Updated(obj.File1);
			if (obj.File2 != null) _FileSystemWatcher_Updated(obj.File2);
		}

		void _FileSystemWatcher_Updated(string FilePath)
		{
			if (UncacheChain.ContainsKey(FilePath))
			{
				foreach (var Item in UncacheChain[FilePath])
				{
					Cache.Remove(Item);
				}
				UncacheChain[FilePath].Clear();
			}

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
			var FilePath = Request.Url.Path;
			if (Path.GetExtension(FilePath) == "")
			{
				if (Directory.Exists(FilePath))
				{
					FilePath = Url.Normalize(FilePath + "/index.html");
				}
			}

			//var CacheKey = FilePath.ToLowerInvariant();
			//var CacheKey = FilePath;
			var CachedResult = await Cache.GetAsync(FilePath, async () =>
			{
				var Extension = Path.GetExtension(FilePath);
				if (ExtensionHandlersAsync.ContainsKey(Extension))
				{
					try
					{
						return await ExtensionHandlersAsync[Extension](new HandlerStruct(this)
						{
							FileSystem = VirtualFileSystem,
							FilePath = FilePath,
						});
					}
					catch (FileNotFoundException)
					{
						throw (new HttpException(HttpCode.NOT_FOUND_404));
					}
				}

				if (Cache.Enabled)
				{
					await Console.Out.WriteLineAsync(String.Format("Caching '{0}'", FilePath));
				}

				var FileInfo = await VirtualFileSystem.GetFileInfoAsync(FilePath);

				if (FileInfo.Exists)
				{
					bool ShouldCacheInMemory = FileInfo.Length <= CacheSizeThresold;
					string ETag = null;
					byte[] Data = null;

					if (ShouldCacheInMemory)
					{
						Data = await VirtualFileSystem.ReadAsBytesAsync(FilePath);
						ETag = BitConverter.ToString(MD5.Create().ComputeHash(Data));
					}

					return new ResultStruct()
					{
						RealFilePath = FilePath,
						ContentType = MimeType.GetFromPath(FilePath),
						FileInfo = FileInfo,
						ETag = ETag,
						Data = Data,
						Exists = true,
					};
				}
				else
				{
					throw (new HttpException(HttpCode.NOT_FOUND_404));
				}
			});

			Response.Headers["Content-Type"] = CachedResult.ContentType;
			if (CachedResult.ETag != null)
			{
				Response.Headers["ETag"] = CachedResult.ETag;
			}
			Response.Headers["Date"] = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
			Response.Headers["Last-Modified"] = CachedResult.FileInfo.LastWriteTimeUtc.ToString("R", CultureInfo.InvariantCulture);
			Response.Headers["Cache-Control"] = "max-age=2419200, private";
			Response.Headers["Expires"] = "Wed, 11 Apr 2022 18:23:41 GMT";

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

			Response.Headers["Content-Length"] = CachedResult.FileInfo.Length.ToString();

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Extension"></param>
		/// <param name="HandlerAsync"></param>
		public void AddExtensionHandler(string Extension, ExtensionHandlerDelegateAsync HandlerAsync)
		{
			ExtensionHandlersAsync["." + Extension] = HandlerAsync;
			//throw new NotImplementedException();
		}

		public void AddFilter(IHttpStaticFilter IHttpStaticFilter)
		{
			IHttpStaticFilter.RegisterFilters(this);
		}
	}
}
