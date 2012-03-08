using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Net.Http.Static
{
	public class HttpStaticFileServer : IHttpFilter
	{
		protected bool Cache;
		protected string Path;

		public HttpStaticFileServer(string Path, bool Cache = false)
		{
			this.Path = Path;
			this.Cache = Cache; // Cache not implemented yet
		}

		async Task IHttpFilter.Filter(HttpRequest Request, HttpResponse Response)
		{
			Response.Buffering = true;
			var FilePath = Url.GetInnerFileRelativeToPath(this.Path, Request.Url);
			if (Directory.Exists(FilePath))
			{
				FilePath = FilePath + "/index.html";
			}
			Response.Headers["Content-Type"] = MimeType.GetFromPath(FilePath);
			await Response.StreamFileASync(FilePath);
		}
	}
}
