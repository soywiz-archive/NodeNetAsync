using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.MongoDb.Bson
{
	public class BsonSerializer
	{
		static public void SerializeDocument(Stream Stream, JsonValue Value)
		{
			new BinaryWriter(Stream).Write((uint)0);
			new BinaryWriter(Stream).Write((uint)0);
			new BinaryWriter(Stream).Write((byte)0);
		}
	}
}
