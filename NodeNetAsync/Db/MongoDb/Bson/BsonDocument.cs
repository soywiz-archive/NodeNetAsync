using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.MongoDb.Bson
{
	public class BsonDocument : BsonValue
	{
		public Dictionary<String, BsonValue> Dictionary = new Dictionary<string, BsonValue>();

		public BsonValue this[string Key]
		{
			get
			{
				return Dictionary[Key];
			}
			set
			{
				Dictionary[Key] = value;
			}
		}

		public override BsonType BsonType { get { return BsonType.Document; } }

		public override void Serialize(Stream Stream)
		{
			var Writer = new BinaryWriter(Stream, Encoding.UTF8);
			var DocumentStartPosition = Stream.Position;
			Writer.Write((uint)0); // Set later
			{
				foreach (var Pair in Dictionary)
				{
					// Type
					Writer.Write((byte)Pair.Value.BsonType);

					// cstring (stringz)
					Writer.Write(Encoding.UTF8.GetBytes(Pair.Key));
					Writer.Write((byte)0);

					// Data
					Pair.Value.Serialize(Stream);
				}
			}
			Writer.Write((byte)0);
			var DocumentEndPosition = Stream.Position;

			Stream.Position = DocumentStartPosition;
			Writer.Write((uint)(DocumentEndPosition - DocumentStartPosition));
			Stream.Position = DocumentEndPosition;
		}
	}
}
