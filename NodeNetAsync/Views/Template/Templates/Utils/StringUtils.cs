﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpUtils.Templates.Utils
{
	static public class StringUtils
	{
		static public String EscapeString(String Value)
		{
			String EscapedString = "";

			if (Value == null)
			{
				Console.Error.WriteLine("EscapeString.Value=null");
			}

			for (int n = 0; n < Value.Length; n++)
			{
				switch (Value[n])
				{
					case '\n': EscapedString += @"\n"; break;
					case '\r': EscapedString += @"\r"; break;
					case '\t': EscapedString += @"\t"; break;
					case '"': EscapedString += "\\\""; break;
					default: EscapedString += Value[n]; break;
				}
			}

			return '"' + EscapedString + '"';
		}

		static public String UnescapeString(String Value)
		{
			if (Value.Length < 2) throw(new Exception("Invalid String [1]"));
			if (Value[0] != '\'' && Value[0] != '"') throw (new Exception("Invalid String [2]"));
			if (Value[0] != Value[Value.Length - 1]) throw (new Exception("Invalid String [3]"));
			String RetString = "";
			Value = Value.Substring(1, Value.Length - 2);
			for (int n = 0; n < Value.Length; n++)
			{
				if (Value[n] == '\\')
				{
					switch (Value[++n])
					{
						case 'n': RetString += '\n'; break;
						case 'r': RetString += '\r'; break;
						case 't': RetString += '\t'; break;
						default: throw(new Exception("Unknown Escape Sequence"));
					}
				}
				else
				{
					RetString += Value[n];
				}
			}

			return RetString;
		}
	}
}
