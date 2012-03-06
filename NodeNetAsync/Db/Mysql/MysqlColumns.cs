using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlColumns : IEnumerable<MysqlField>
	{
		private List<MysqlField> ColumnsByIndex = new List<MysqlField>();
		private Dictionary<string, MysqlField> ColumnsByName = new Dictionary<string, MysqlField>();
		private Dictionary<string, int> ColumnIndexByName = new Dictionary<string, int>();

		public void Add(MysqlField Column)
		{
			ColumnsByIndex.Add(Column);
			ColumnsByName.Add(Column.Name, Column);
			ColumnIndexByName.Add(Column.Name, ColumnsByIndex.Count - 1);
		}

		public MysqlField this[int Index]
		{
			get
			{
				return ColumnsByIndex[Index];
			}
		}

		public MysqlField this[string Name]
		{
			get
			{
				return ColumnsByName[Name];
			}
		}

		public int GetIndexByColumnName(string Name)
		{
			return ColumnIndexByName[Name];
		}

		public int Length { get { return ColumnsByIndex.Count; } }

		public IEnumerator<MysqlField> GetEnumerator()
		{
			return ColumnsByIndex.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ColumnsByIndex.GetEnumerator();
		}
	}
}
