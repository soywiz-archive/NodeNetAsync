using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.Static
{
	public interface IHttpStaticFilter
	{
		void RegisterFilters(HttpStaticFileServer HttpStaticFileServer);
	}
}
