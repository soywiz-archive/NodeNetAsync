using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mongo
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://www.mongodb.org/display/DOCS/Mongo+Wire+Protocol"/>
	public class MongoClient
	{
		const ushort DefaultPort = 27017;

		public MongoClient(ushort Port = DefaultPort)
		{
		}
	}

	public enum UpdateFlagsSet : uint
	{
		/// <summary>
		/// If set, the database will insert the supplied object into the collection if no matching document is found.
		/// </summary>
		Upsert = (1 << 0),

		/// <summary>
		/// If set, the database will update all matching objects in the collection. Otherwise only updates first matching doc.
		/// </summary>
		MultiUpdate = (1 << 1),

		//2-31	 Reserved	 Must be set to 0.
	}

	public enum OpcodeEnum : uint
	{
		/// <summary>
		/// Reply to a client request. responseTo is set
		/// </summary>
		OP_REPLY	    = 1,
		
		/// <summary>
		/// generic msg command followed by a string
		/// </summary>
		OP_MSG	        = 1000,
		
		/// <summary>
		/// update document
		/// </summary>
		OP_UPDATE	    = 2001,
		
		/// <summary>
		/// insert new document
		/// </summary>
		OP_INSERT	    = 2002,
		
		/// <summary>
		/// formerly used for OP_GET_BY_OID
		/// </summary>
		RESERVED	    = 2003,
		
		/// <summary>
		/// query a collection
		/// </summary>
		OP_QUERY	    = 2004,
		
		/// <summary>
		/// Get more data from a query. See Cursors
		/// </summary>
		OP_GETMORE	    = 2005,
		
		/// <summary>
		/// Delete documents
		/// </summary>
		OP_DELETE	    = 2006,
		
		/// <summary>
		/// Tell database client is done with a cursor
		/// </summary>
		OP_KILL_CURSORS	= 2007,
	}
}
