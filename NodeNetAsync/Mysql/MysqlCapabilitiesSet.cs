using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Mysql
{
	public enum MysqlCapabilitiesSet : uint
	{
		/// <summary>
		/// new more secure passwords
		/// </summary>
		CLIENT_LONG_PASSWORD = 1,

		/// <summary>
		/// Found instead of affected rows 
		/// </summary>
		CLIENT_FOUND_ROWS = 2,

		/// <summary>
		/// Get all column flags 
		/// </summary>
		CLIENT_LONG_FLAG = 4,

		/// <summary>
		/// One can specify db on connect 
		/// </summary>
		CLIENT_CONNECT_WITH_DB = 8,

		/// <summary>
		/// Don't allow database.table.column 
		/// </summary>
		CLIENT_NO_SCHEMA = 16,

		/// <summary>
		/// Can use compression protocol 
		/// </summary>
		CLIENT_COMPRESS = 32,

		/// <summary>
		/// Odbc client
		/// </summary>
		CLIENT_ODBC = 64,

		/// <summary>
		/// Can use LOAD DATA LOCAL
		/// </summary>
		CLIENT_LOCAL_FILES = 128,

		/// <summary>
		/// Ignore spaces before '(' 
		/// </summary>
		CLIENT_IGNORE_SPACE = 256,

		/// <summary>
		/// New 4.1 protocol
		/// </summary>
		CLIENT_PROTOCOL_41 = 512,

		/// <summary>
		/// This is an interactive client
		/// </summary>
		CLIENT_INTERACTIVE = 1024,

		/// <summary>
		/// Switch to SSL after handshake
		/// </summary>
		CLIENT_SSL = 2048,

		/// <summary>
		/// IGNORE sigpipes
		/// </summary>
		CLIENT_IGNORE_SIGPIPE = 4096,

		/// <summary>
		/// Client knows about transactions
		/// </summary>
		CLIENT_TRANSACTIONS = 8192,

		/// <summary>
		/// Old flag for 4.1 protocol
		/// </summary>
		CLIENT_RESERVED = 16384,

		/// <summary>
		/// New 4.1 authentication
		/// </summary>
		CLIENT_SECURE_CONNECTION = 32768,

		/// <summary>
		/// Enable/disable multi-stmt support
		/// </summary>,
		CLIENT_MULTI_STATEMENTS = 65536,

		/// <summary>
		/// Enable/disable multi-results
		/// </summary>
		CLIENT_MULTI_RESULTS = 131072,

		DEFAULT =
			CLIENT_LONG_PASSWORD
			| CLIENT_FOUND_ROWS
			| CLIENT_LONG_FLAG
			| CLIENT_CONNECT_WITH_DB
			| CLIENT_ODBC
			| CLIENT_LOCAL_FILES
			| CLIENT_IGNORE_SPACE
			| CLIENT_PROTOCOL_41
			| CLIENT_INTERACTIVE
			| CLIENT_IGNORE_SIGPIPE
			| CLIENT_TRANSACTIONS
			| CLIENT_RESERVED
			| CLIENT_SECURE_CONNECTION
			| CLIENT_MULTI_STATEMENTS
			| CLIENT_MULTI_RESULTS
		,
	}
}
