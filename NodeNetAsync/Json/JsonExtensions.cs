using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Json
{
	static public class JsonExtensions
	{
		static public JsonValue ToJson<TType>(this TType Object)
		{
			return JsonSerializer.Serialize(Object);
		}
	}
}
