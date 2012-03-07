using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SystemDns = System.Net.Dns;

namespace NodeNetAsync.OS
{
	public class Dns
	{
		async static public Task<IPHostEntry> ResolveAsync(string HostName)
		{
			return await SystemDns.GetHostEntryAsync(HostName);
		}
	}
}
