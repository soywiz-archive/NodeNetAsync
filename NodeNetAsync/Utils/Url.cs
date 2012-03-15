using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class Url
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Url"></param>
		/// <returns></returns>
		static protected string _Normalize(string Url)
		{
			return Url.Replace('\\', '/');
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Url"></param>
		/// <returns></returns>
		static private string ExpandDirectoriesInternal(string Url)
		{
			var Components = new List<string>();
			//Console.WriteLine("{0} : {1}", Url, Url.Split('/').Length);
			foreach (var Component in Url.Split('/'))
			{
				switch (Component)
				{
					case "":
					case ".":
						break;
					case "..": if (Components.Count > 0) Components.RemoveAt(Components.Count - 1); break;
					default: Components.Add(Component); break;
				}
			}

			return String.Join("/", Components);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Url"></param>
		/// <returns></returns>
		static public string Normalize(string Url)
		{
			Url = _Normalize(Url);
			if (Url.StartsWith("/"))
			{
				return "/" + ExpandDirectoriesInternal(Url.Substring(1).TrimStart('/'));
			}
			else
			{
				return ExpandDirectoriesInternal(Url);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Base"></param>
		/// <param name="Relative"></param>
		/// <returns></returns>
		static public string GetInnerFileRelativeToPath(string Base, string Relative)
		{
			Base = Normalize(Base);
			Relative = Normalize(Relative);
			return Base.TrimEnd('/') + "/" + Relative.TrimStart('/');
		}
	}
}
