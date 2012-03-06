using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class Url
	{
		static public string Normalize(string Url)
		{
			return Url.Replace('\\', '/');
		}

		static public string ExpandDirectories(string Url)
		{
			var Components = new List<string>();
			Url = Normalize(Url);
			foreach (var Component in Url.Split('/'))
			{
				switch (Component)
				{
					case "":
					case ".":
						if (Components.Count == 0) Components.Add("");
					break;
					case "..": if (Components.Count > 0) Components.RemoveAt(Components.Count - 1); break;
					default: Components.Add(Component); break;
				}
			}

			return String.Join("/", Components);
		}

		static public string GetInnerFileRelativeToPath(string Base, string Relative)
		{
			Base = ExpandDirectories(Normalize(Base));
			Relative = ExpandDirectories(Normalize(Relative));
			return Base + "/" + Relative;
		}
	}
}
