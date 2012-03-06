using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class HttpHeaders : IEnumerable<HttpHeader>
	{
		public List<HttpHeader> List = new List<HttpHeader>();

		public IEnumerator<HttpHeader> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		public void Add(string Key, string Value, bool Replace = false)
		{
			List.Add(new HttpHeader() { Key = Key, Value = Value });
		}

		public string this[string Key]
		{
			get
			{
				return List.Where(Item => Item.Key == Key).Select(Item => Item.Key).First();
			}
			set
			{
				Add(Key, value, true);
			}
		}

		public override string ToString()
		{
			return String.Join("\r\n", List) + "\r\n";
		}
	}
}
