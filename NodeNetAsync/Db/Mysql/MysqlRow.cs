using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlRow
	{
		public MysqlColumns MysqlColumns;
		public List<string> Cells = new List<string>();

		public MysqlRow(MysqlColumns MysqlColumns)
		{
			this.MysqlColumns = MysqlColumns;
		}

		public string this[int Index]
		{
			get
			{
				return Cells[Index];
			}
		}

		public string this[string Name]
		{
			get
			{
				return this[MysqlColumns.GetIndexByColumnName(Name)];
			}
		}

		public override string ToString()
		{
			var Parts = new List<string>();
			for (int n = 0; n < Cells.Count; n++)
			{
				Parts.Add("'" + MysqlColumns[n].Name + "': '" + Cells[n] + "'");
			}
			return "MysqlRow(" + String.Join(", ", Parts) + ")";
		}
	}
}
