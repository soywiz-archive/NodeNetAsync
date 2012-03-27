using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.MongoDb.Bson
{
	public abstract class BsonValue
	{
		abstract public BsonType BsonType { get; }
		abstract public void Serialize(Stream Stream);
		public byte[] Serialize()
		{
			var MemoryStream = new MemoryStream();
			Serialize(MemoryStream);
			//MemoryStream.GetContentBytes
			return MemoryStream.ToArray();
		}
	}
}
