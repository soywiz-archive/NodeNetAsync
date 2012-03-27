using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.CouchDb
{
	public class CouchDbCollection
	{
		CouchDbClient Client;
		string Name;

		internal CouchDbCollection(CouchDbClient Client, string Name)
		{
			this.Client = Client;
			this.Name = Name;
		}

		async public Task CreateAsync()
		{
			try
			{
				var Result = await Client.Raw.SendAsync(Name, "PUT");
			}
			catch (WebException)
			{
				throw (new Exception("Can't create database '" + Name + "'"));
			}
		}
	}
}
