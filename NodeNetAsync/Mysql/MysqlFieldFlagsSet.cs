using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Mysql
{
	/// <summary>
	/// The possible flag values at time of
	/// writing (taken from  include/mysql_com.h), in hexadecimal:
	/// </summary>
	public enum MysqlFieldFlagsSet : ushort
	{
		NOT_NULL_FLAG		 = 0x0001, 
		PRI_KEY_FLAG		 = 0x0002, 
		UNIQUE_KEY_FLAG		 = 0x0004, 
		MULTIPLE_KEY_FLAG	 = 0x0008, 
		BLOB_FLAG			 = 0x0010, 
		UNSIGNED_FLAG		 = 0x0020, 
		ZEROFILL_FLAG		 = 0x0040, 
		BINARY_FLAG			 = 0x0080, 
		ENUM_FLAG			 = 0x0100, 
		AUTO_INCREMENT_FLAG	 = 0x0200, 
		TIMESTAMP_FLAG		 = 0x0400, 
		SET_FLAG			 = 0x0800, 
	}
}
