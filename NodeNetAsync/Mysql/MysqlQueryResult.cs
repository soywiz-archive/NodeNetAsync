using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Mysql
{
	public class MysqlQueryResult : IEnumerable<MysqlRow>
	{
		public MysqlColumns Columns = new MysqlColumns();
		public List<MysqlRow> Rows = new List<MysqlRow>();

		public IEnumerator<MysqlRow> GetEnumerator()
		{
			return Rows.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Rows.GetEnumerator();
		}
	}
}
