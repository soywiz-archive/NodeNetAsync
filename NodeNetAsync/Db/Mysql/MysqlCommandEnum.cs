using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public enum MysqlCommandEnum : byte
	{
		/// <summary>
		/// (none, this is an internal thread state)
		/// </summary>
		COM_SLEEP              = 0x00,

		/// <summary>
		/// mysql_close
		/// </summary>
		COM_QUIT               = 0x01,
		
		/// <summary>
		/// mysql_select_db 
		/// </summary>
		COM_INIT_DB            = 0x02,
		
		/// <summary>
		/// mysql_real_query
		/// </summary>
		COM_QUERY              = 0x03,
		
		/// <summary>
		/// mysql_list_fields
		/// </summary>
		COM_FIELD_LIST         = 0x04,
		
		/// <summary>
		/// mysql_create_db (deprecated)
		/// </summary>
		COM_CREATE_DB          = 0x05,
		
		/// <summary>
		/// mysql_drop_db (deprecated)
		/// </summary>
		COM_DROP_DB            = 0x06,
		
		/// <summary>
		/// mysql_refresh
		/// </summary>
		COM_REFRESH            = 0x07,
		
		/// <summary>
		/// mysql_shutdown
		/// </summary>
		COM_SHUTDOWN           = 0x08,
		
		/// <summary>
		/// mysql_stat
		/// </summary>
		COM_STATISTICS         = 0x09,
		
		/// <summary>
		/// mysql_list_processes
		/// </summary>
		COM_PROCESS_INFO       = 0x0a,
		
		/// <summary>
		/// (none, this is an internal thread state)
		/// </summary>
		COM_CONNECT            = 0x0b,
		
		/// <summary>
		/// mysql_kill
		/// </summary>
		COM_PROCESS_KILL       = 0x0c,
		
		/// <summary>
		/// mysql_dump_debug_info
		/// </summary>
		COM_DEBUG              = 0x0d,
		
		/// <summary>
		/// mysql_ping
		/// </summary>
		COM_PING               = 0x0e,
		
		/// <summary>
		/// (none, this is an internal thread state)
		/// </summary>
		COM_TIME               = 0x0f,
		
		/// <summary>
		/// (none, this is an internal thread state)
		/// </summary>
		COM_DELAYED_INSERT     = 0x10,
		
		/// <summary>
		/// mysql_change_user
		/// </summary>
		COM_CHANGE_USER        = 0x11,
		
		/// <summary>
		/// sent by the slave IO thread to request a binlog
		/// </summary>
		COM_BINLOG_DUMP        = 0x12,
		
		/// <summary>
		/// LOAD TABLE ... FROM MASTER (deprecated)
		/// </summary>
		COM_TABLE_DUMP         = 0x13,
		
		/// <summary>
		/// (none, this is an internal thread state)
		/// </summary>
		COM_CONNECT_OUT        = 0x14,
		
		/// <summary>
		/// sent by the slave to register with the master (optional)
		/// </summary>
		COM_REGISTER_SLAVE     = 0x15,
		
		/// <summary>
		/// mysql_stmt_prepare
		/// </summary>
		COM_STMT_PREPARE       = 0x16,
		
		/// <summary>
		/// mysql_stmt_execute
		/// </summary>
		COM_STMT_EXECUTE       = 0x17,
		
		/// <summary>
		/// mysql_stmt_send_long_data
		/// </summary>
		COM_STMT_SEND_LONG_DATA= 0x18,
		
		/// <summary>
		/// mysql_stmt_close
		/// </summary>
		COM_STMT_CLOSE         = 0x19,
		
		/// <summary>
		/// mysql_stmt_reset
		/// </summary>
		COM_STMT_RESET         = 0x1a,
		
		/// <summary>
		/// mysql_set_server_option
		/// </summary>
		COM_SET_OPTION         = 0x1b,
		
		/// <summary>
		/// mysql_stmt_fetch
		/// </summary>
		COM_STMT_FETCH         = 0x1c,
	}
}
