using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.CouchDb
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://wiki.apache.org/couchdb/Complete_HTTP_API_Reference"/>
	public partial class CouchDbClient
	{
		const ushort DefaultPort = 5984;

		public readonly string Base = "http://127.0.0.1:5984";
		public readonly CouchDbRaw Raw;

		public CouchDbClient(string Host = "127.0.0.1", ushort Port = DefaultPort)
		{
			this.Base = "http://" + Host + ":" + Port;
			this.Raw = new CouchDbRaw(this);
		}
	}
}
