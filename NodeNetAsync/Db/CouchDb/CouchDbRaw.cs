using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NodeNetAsync.Db.CouchDb
{
	public class CouchDbRaw
	{
		CouchDbClient Client;

		internal CouchDbRaw(CouchDbClient Client)
		{
			this.Client = Client;
		}

		async public Task<JObject> SendAsync(string Path, string Method, string Data = "", Tuple<string, string>[] RequestHeaders = null)
		{
			var WebClient = new WebClient();
			WebClient.Proxy = null;
			if (RequestHeaders != null)
			{
				foreach (var Header in RequestHeaders) WebClient.Headers.Add(Header.Item1, Header.Item2);
			}
			if (Method == "GET" || Method == "HEAD")
			{
				return JObject.Parse(await WebClient.DownloadStringTaskAsync(Client.Base + "/" + Path));
			}
			else
			{
				return JObject.Parse(await WebClient.UploadStringTaskAsync(Client.Base + "/" + Path, Method, Data));
			}
		}

		async public Task<JObject> CopyAsync(string PathSource, string PathDestination)
		{
			return await SendAsync(PathSource, "COPY", "", new[] { new Tuple<string, string>("Destination", PathDestination) });
		}

		async public Task<JObject> PutAsync(string Path, JObject Data)
		{
			return await SendAsync(Path, "PUT", Data.ToString());
		}

		async public Task<JObject> DeleteAsync(string Path, string Data)
		{
			return await SendAsync(Path, "DELETE");
		}

		async public Task<JObject> GetAsync(string Path)
		{
			return await SendAsync(Path, "GET");
		}

		async public Task<JObject> HeadAsync(string Path)
		{
			return await SendAsync(Path, "HEAD");
		}
	}
}
