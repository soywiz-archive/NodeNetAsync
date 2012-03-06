using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlField
	{
		public string Catalog;
		public string Database;
		public string Table;
		public string OrgTable;
		public string Name;
		public string OrgName;
		public ushort Charset;
		public uint Length;
		public MysqlFieldTypeEnum Type;
		public MysqlFieldFlagsSet Flags;
		public byte Decimals;
		public byte[] Default;
	}
}
