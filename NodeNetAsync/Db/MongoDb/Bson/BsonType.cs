using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.MongoDb.Bson
{
	public enum BsonType : byte
	{
		/// <summary>
		/// "\x01" e_name double	Floating point
		/// </summary>
		FloatingPoint = 0x01,
		
		/// <summary>
		/// "\x02" e_name string	UTF-8 string
		/// </summary>
		Utf8String = 0x02,
		
		/// <summary>
		/// "\x03" e_name document	Embedded document
		/// </summary>
		Document = 0x03,
		
		/// <summary>
		/// "\x04" e_name document	Array
		/// </summary>
		Array = 0x04,
		
		/// <summary>
		/// "\x05" e_name binary	Binary data
		/// </summary>
		BinaryData = 0x05,
		
		/// <summary>
		/// "\x06" e_name	Undefined — Deprecated
		/// </summary>
		Undefined = 0x06,
		
		/// <summary>
		/// "\x07" e_name (byte*12)	ObjectId
		/// </summary>
		ObjectId = 0x07,
		
		/// <summary>
		/// "\x08" e_name "\x00"	Boolean "false"
		/// </summary>
		Boolean = 0x08,

		/// <summary>
		/// "\x09" e_name int64	UTC datetime
		/// </summary>
		UtcDatetime = 0x09,
		
		/// <summary>
		/// "\x0A" e_name	Null value
		/// </summary>
		Null = 0x0A,

		/// <summary>
		/// "\x0B" e_name cstring cstring	Regular expression
		/// </summary>
		RegularExpression = 0x0B,
		
		/// <summary>
		/// "\x0C" e_name string (byte*12)	DBPointer — Deprecated
		/// </summary>
		DBPointer = 0x0C,
		
		/// <summary>
		/// "\x0D" e_name string	JavaScript code
		/// </summary>
		JavascriptCode = 0x0D,
		
		/// <summary>
		/// "\x0E" e_name string	Symbol
		/// </summary>
		Symbol = 0x0E,
		
		/// <summary>
		/// "\x0F" e_name code_w_s	JavaScript code w/ scope
		/// </summary>
		JavaScriptCodeWithScope = 0x0F,

		/// <summary>
		/// "\x10" e_name int32	32-bit Integer
		/// </summary>
		Int32 = 0x10,
		
		/// <summary>
		/// "\x11" e_name int64	Timestamp
		/// </summary>
		Timestamp = 0x11,

		/// <summary>
		/// "\x12" e_name int64	64-bit integer
		/// </summary>
		Int64 = 0x12,
		
		/// <summary>
		/// "\xFF" e_name	Min key
		/// </summary>
		MinKey = 0xFF,
		
		/// <summary>
		/// "\x7F" e_name	Max key
		/// </summary>
		MaxKey = 0x7F,
	}
}
