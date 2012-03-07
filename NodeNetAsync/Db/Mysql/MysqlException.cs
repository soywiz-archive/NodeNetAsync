using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlException : Exception
	{
		public readonly int ErrorCode;
		public readonly string SqlState;
		public readonly string MysqlMessage;

		public MysqlException(int ErrorCode, string SqlState, string Message) : base (ErrorCode + " : " + SqlState + ":" + Message)
		{
			this.ErrorCode = ErrorCode;
			this.SqlState = SqlState;
			this.MysqlMessage = Message;
		}
	}
}
