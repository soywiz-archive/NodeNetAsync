using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.MongoDb.Bson
{
	public class BsonArray : BsonValue
	{
		public override BsonType BsonType
		{
			get { return Bson.BsonType.Array; }
		}

		public override void Serialize(System.IO.Stream Stream)
		{
			throw new NotImplementedException();
		}
	}
}
