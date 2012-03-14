using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class HttpHeaders : IEnumerable<HttpHeader>
	{
		protected List<HttpHeader> List = new List<HttpHeader>();
		protected Dictionary<string, int> CurrentItems = new Dictionary<string, int>();

		public IEnumerator<HttpHeader> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		protected void AddOrSet(HttpHeader HttpHeader, bool Replace)
		{
			if (Replace && CurrentItems.ContainsKey(HttpHeader.NormalizedKey))
			{
				List[CurrentItems[HttpHeader.NormalizedKey]] = HttpHeader;
			}
			else
			{
				List.Add(HttpHeader);
				CurrentItems[HttpHeader.NormalizedKey] = List.Count - 1;
			}
		}

		public void Add(HttpHeader HttpHeader)
		{
			AddOrSet(HttpHeader, Replace: false);
		}
		
		public void Add(string Key, string Value)
		{
			AddOrSet(new HttpHeader(Key, Value), Replace: false);
		}

		public int Count
		{
			get
			{
				return List.Count;
			}
		}

		public void Set(string Key, string Value)
		{
			AddOrSet(new HttpHeader(Key, Value), Replace: true);
		}

		public string this[string Key]
		{
			get
			{
				var NormalizedKey = HttpHeader.NormalizeKey(Key);
				if (CurrentItems.ContainsKey(NormalizedKey))
				{
					return List[CurrentItems[NormalizedKey]].Value;
				}
				else
				{
					return "";
				}
			}
			set
			{
				Set(Key, value);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}

		public string GetEncodeString()
		{
			var StringBuilder = new StringBuilder();
			foreach (var Header in List) StringBuilder.Append(Header.GetEncodeString());
			return StringBuilder.ToString();
		}
	}
}
