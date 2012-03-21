using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Mysql
{
	public partial class MysqlClient
	{
		public string Quote(object Param)
		{
			if (Param == null) return "NULL";

			var Out = new StringBuilder();

			Out.Append("'");

			foreach (var Char in Param.ToString())
			{
				switch (Char)
				{
					case '"': Out.Append("\\\""); break;
					case '\'': Out.Append("\\'"); break;
					case '\\': Out.Append("\\\\"); break;
					default: Out.Append(Char); break;
				}
			}

			Out.Append("'");

			return Out.ToString();
		}

		private string ReplaceParameters(string Query, params object[] Params)
		{
			if (Params.Length == 0)
			{
				return Query;
			}
			else
			{
				int ParamIndex = 0;
				var Out = new StringBuilder();

				foreach (var Char in Query)
				{
					if (Char == '?')
					{
						Out.Append(Quote(Params[ParamIndex]));
						ParamIndex++;
					}
					else
					{
						Out.Append(Char);
					}
				}

				//Console.WriteLine("---{0}", Out.ToString());

				return Out.ToString();
			}
		}
	}
}
