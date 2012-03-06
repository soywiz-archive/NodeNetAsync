using System;
using System.Collections;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Json
{
	public class JsonSerializer
	{
		static public JsonValue Serialize(object Object)
		{
			var Type = Object.GetType();
			if (Type.IsPrimitive)
			{
				if (Type == typeof(bool)) return new JsonPrimitive((bool)(object)Object);
				if (Type == typeof(int)) return new JsonPrimitive((int)(object)Object);
				throw (new NotImplementedException("Can't handle primitive type '" + Type + "' + " + Object + ""));
			}
			if (Type.IsArray || Type.IsSubclassOf(typeof(IEnumerable)))
			{
				return new JsonArray(((IEnumerable)(object)Object).Cast<object>().Select(Item => Serialize(Item)));
			}
			if (Type == typeof(String)) return new JsonPrimitive((string)(object)Object);

			var ObjectItems = new List<KeyValuePair<string, JsonValue>>();
			foreach (var Field in Type.GetFields())
			{
				ObjectItems.Add(new KeyValuePair<string,JsonValue>(Field.Name, Serialize(Field.GetValue(Object))));
			}
			return new JsonObject(ObjectItems);
		}
	}
}
