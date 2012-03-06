using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlRow
	{
		public MysqlColumns Columns;
		public List<string> Cells = new List<string>();

		public MysqlRow(MysqlColumns Columns)
		{
			this.Columns = Columns;
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
				return this[Columns.GetIndexByColumnName(Name)];
			}
		}

		public TType CastTo<TType>()
		{
			var ItemValue = default(TType);
			var ItemType = typeof(TType);

			for (int n = 0; n < Columns.Length; n++)
			{
				var Column = Columns[n];
				var Value = Cells[n];
				var Field = ItemType.GetField(Column.Name);
				if (Field != null)
				{
					object ValueObject = null;
					if (Field.FieldType == typeof(bool))
					{
						ValueObject = (int.Parse(Value) != 0);
					}
					else
					{
						throw(new NotImplementedException("Can't handle type '" + Field.FieldType + "'"));
					}
					Field.SetValueDirect(__makeref(ItemValue), ValueObject);
				}
			}

			return ItemValue;
		}

		public override string ToString()
		{
			var Parts = new List<string>();
			for (int n = 0; n < Cells.Count; n++)
			{
				Parts.Add("'" + Columns[n].Name + "': '" + Cells[n] + "'");
			}
			return "MysqlRow(" + String.Join(", ", Parts) + ")";
		}
	}
}
