using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.CouchDb
{
	public class CouchDbRaw
	{
		CouchDbClient Client;

		internal CouchDbRaw(CouchDbClient Client)
		{
			this.Client = Client;
		}

		async public Task<JsonValue> SendAsync(string Path, string Method, string Data = "", Tuple<string, string>[] RequestHeaders = null)
		{
			var WebClient = new WebClient();
			WebClient.Proxy = null;
			if (RequestHeaders != null)
			{
				foreach (var Header in RequestHeaders) WebClient.Headers.Add(Header.Item1, Header.Item2);
			}
			if (Method == "GET" || Method == "HEAD")
			{
				return JsonObject.Parse(await WebClient.DownloadStringTaskAsync(Client.Base + "/" + Path));
			}
			else
			{
				return JsonObject.Parse(await WebClient.UploadStringTaskAsync(Client.Base + "/" + Path, Method, Data));
			}
		}

		async public Task<JsonValue> CopyAsync(string PathSource, string PathDestination)
		{
			return await SendAsync(PathSource, "COPY", "", new[] { new Tuple<string, string>("Destination", PathDestination) });
		}

		async public Task<JsonValue> PutAsync(string Path, JsonValue Data)
		{
			return await SendAsync(Path, "PUT", Data.ToString());
		}

		async public Task<JsonValue> DeleteAsync(string Path, string Data)
		{
			return await SendAsync(Path, "DELETE");
		}

		async public Task<JsonValue> GetAsync(string Path)
		{
			return await SendAsync(Path, "GET");
		}

		async public Task<JsonValue> HeadAsync(string Path)
		{
			return await SendAsync(Path, "HEAD");
		}
	}
}
