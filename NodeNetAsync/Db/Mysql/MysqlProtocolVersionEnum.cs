using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public enum MysqlProtocolVersionEnum : byte
	{
		Version0 = 0,
		Version10 = 10,
		Error = 0xFF,
	}
}
