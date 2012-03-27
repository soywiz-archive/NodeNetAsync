using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.CouchDb
{
	public partial class CouchDbClient
	{
		public CouchDbCollection GetCollection(string Name)
		{
			return new CouchDbCollection(this, Name);
		}

		async public Task<IEnumerable<string>> GetAllCollectionNamesAsync()
		{
			return ((await Raw.GetAsync("_all_dbs")) as IEnumerable<JsonValue>).Select(Item => (string)Item);
		}

		async public Task<string> GetVersionAsync()
		{
			return (await Raw.GetAsync(""))["version"].ToString();
		}
	}
}
