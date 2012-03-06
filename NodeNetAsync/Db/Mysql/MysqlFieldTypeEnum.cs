using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	/// <summary>
	/// The code for the column's data type. Also known as
    /// "enum_field_type". The possible values at time of
    /// writing (taken from  include/mysql_com.h), in hexadecimal:
	/// </summary>
	public enum MysqlFieldTypeEnum : byte
	{
		FIELD_TYPE_DECIMAL						   = 0x00,   
		FIELD_TYPE_TINY							   = 0x01,   
		FIELD_TYPE_SHORT						   = 0x02,   
		FIELD_TYPE_LONG							   = 0x03,   
		FIELD_TYPE_FLOAT						   = 0x04,   
		FIELD_TYPE_DOUBLE						   = 0x05,   
		FIELD_TYPE_NULL							   = 0x06,   
		FIELD_TYPE_TIMESTAMP					   = 0x07,   
		FIELD_TYPE_LONGLONG						   = 0x08,   
		FIELD_TYPE_INT24						   = 0x09,   
		FIELD_TYPE_DATE							   = 0x0a,   
		FIELD_TYPE_TIME							   = 0x0b,   
		FIELD_TYPE_DATETIME						   = 0x0c,   
		FIELD_TYPE_YEAR							   = 0x0d,   
		FIELD_TYPE_NEWDATE						   = 0x0e,   
		FIELD_TYPE_VARCHAR 	   = 0x0f,    // (new in MySQL 5.0)
		FIELD_TYPE_BIT 		   = 0x10,    // (new in MySQL 5.0)
		FIELD_TYPE_NEWDECIMAL = 0xf6,    // (new in MYSQL 5.0)
		FIELD_TYPE_ENUM							   = 0xf7,   
		FIELD_TYPE_SET							   = 0xf8,   
		FIELD_TYPE_TINY_BLOB					   = 0xf9,   
		FIELD_TYPE_MEDIUM_BLOB					   = 0xfa,   
		FIELD_TYPE_LONG_BLOB					   = 0xfb,   
		FIELD_TYPE_BLOB							   = 0xfc,   
		FIELD_TYPE_VAR_STRING					   = 0xfd,   
		FIELD_TYPE_STRING						   = 0xfe,   
		FIELD_TYPE_GEOMETRY						   = 0xff,   
	}
}
