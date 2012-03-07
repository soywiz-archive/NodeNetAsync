using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlRow : IEnumerable<KeyValuePair<string, string>>, IDictionary
	{
		public MysqlColumns Columns;
		public List<string> Cells = new List<string>();

		public MysqlRow(MysqlColumns Columns)
		{
			this.Columns = Columns;
		}

		public string this[string Name]
		{
			get
			{
				return this[Columns.GetIndexByColumnName(Name)];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string this[int Index]
		{
			get
			{
				return Cells[Index];
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

		IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
		{
			for (int n = 0; n < Columns.Length; n++)
			{
				var Column = Columns[n];
				var Value = Cells[n];
				yield return new KeyValuePair<string, string>(Column.Name, Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var Item in ((IEnumerable<KeyValuePair<string, string>>)this).AsEnumerable()) yield return Item;
		}

		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(object key)
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public bool IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public ICollection Keys
		{
			get { throw new NotImplementedException(); }
		}

		public void Remove(object key)
		{
			throw new NotImplementedException();
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public object this[object key]
		{
			get
			{
				return this[Columns.GetIndexByColumnName((string)key)];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return Columns.Length; }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public struct Enumerator : IDictionaryEnumerator
		{
			private MysqlRow Row;
			private int CurrentIndex;

			internal Enumerator(MysqlRow Row)
			{
				this.Row = Row;
				this.CurrentIndex = -1;
			}

			public DictionaryEntry Entry
			{
				get { return new DictionaryEntry(Key, Value); }
			}

			public object Key
			{
				get { return this.Row.Columns[CurrentIndex].Name; }
			}

			public object Value
			{
				get { return this.Row.Cells[CurrentIndex]; }
			}

			public object Current
			{
				get { throw new NotImplementedException(); }
			}

			public bool MoveNext()
			{
				return ++this.CurrentIndex < Row.Count;
			}

			public void Reset()
			{
				this.CurrentIndex = -1;
			}
		}
	}
}
