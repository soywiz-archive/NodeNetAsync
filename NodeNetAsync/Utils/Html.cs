using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class Html
	{
		// @TODO
		static public string Quote(string Input)
		{
			var Output = new StringBuilder();
			foreach (var Char in Input)
			{
				switch (Char)
				{
					case '&': Output.Append("&amp;"); break;
					case '<': Output.Append("&lt;"); break;
					case '>': Output.Append("&gt;"); break;
					case '"': Output.Append("&quot;"); break;
					case '\\': Output.Append("&#39;"); break;
					default: Output.Append(Char); break;
				}
			}
			return Output.ToString();
			//return Input.Replace(;
		}
	}
}
