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
			
			// Primitive
			if (Type.IsPrimitive)
			{
				if (Type == typeof(bool)) return new JsonPrimitive((bool)(object)Object);
				if (Type == typeof(int)) return new JsonPrimitive((int)(object)Object);
				throw (new NotImplementedException("Can't handle primitive type '" + Type + "' + " + Object + ""));
			}

			// Array
			if (Type.IsArray || Type.IsSubclassOf(typeof(IEnumerable)))
			{
				return new JsonArray(((IEnumerable)(object)Object).Cast<object>().Select(Item => Serialize(Item)));
			}

			// String
			if (Type == typeof(String))
			{
				return new JsonPrimitive((string)(object)Object);
			}

			// IDictionary
			if (Type.GetInterfaces().Any(InterfaceType => InterfaceType == typeof(IDictionary)))
			{
				var Enumerator = ((IDictionary)Object).GetEnumerator();
				var Items = new Dictionary<string, JsonValue>();
				while (Enumerator.MoveNext())
				{
					Items.Add((string)Enumerator.Key, Serialize(Enumerator.Value));
				}
				return new JsonObject(Items);
			}

			// IEnumerable
			if (Type.GetInterfaces().Any(InterfaceType => InterfaceType == typeof(IEnumerable)))
			{
				var Enumerator = ((IEnumerable)Object).GetEnumerator();
				var List = new List<JsonValue>();
				while (Enumerator.MoveNext())
				{
					List.Add(Serialize(Enumerator.Current));
				}
				return new JsonArray(List);
			}

			// Any other class/struct
			var ObjectItems = new List<KeyValuePair<string, JsonValue>>();
			foreach (var Field in Type.GetFields())
			{
				ObjectItems.Add(new KeyValuePair<string,JsonValue>(Field.Name, Serialize(Field.GetValue(Object))));
			}
			return new JsonObject(ObjectItems);
		}
	}
}
