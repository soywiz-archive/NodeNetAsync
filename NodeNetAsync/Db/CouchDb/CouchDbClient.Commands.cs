using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
			var Value = await Raw.GetAsync("_all_dbs");
			return Value.ToObject<string[]>();
		}

		async public Task<string> GetVersionAsync()
		{
			return (await Raw.GetAsync(""))["version"].ToString();
		}
	}
}
