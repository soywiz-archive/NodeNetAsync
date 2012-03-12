using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net
{
	public partial class TcpServer
	{
		/// <summary>
		/// Returns first available port on the specified IP address. The port scan excludes ports that are open on ANY loopback adapter. 
		/// If the address upon which a port is requested is an 'ANY' address all ports that are open on ANY IP are excluded.
		/// </summary>
		/// <param name="RangeStart"></param>
		/// <param name="RangeEnd"></param>
		/// <param name="BindIp">The IP address upon which to search for available port.</param>
		/// <param name="IncludeIdlePorts">If true includes ports in TIME_WAIT state in results. TIME_WAIT state is typically cool down period for recently released ports.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public static ushort GetAvailablePort(ushort RangeStart = 1025, ushort RangeEnd = ushort.MaxValue, IPAddress BindIp = null, bool IncludeIdlePorts = false)
		{
			if (BindIp == null) BindIp = IPAddress.Parse("127.0.0.1");

			var IpProperties = IPGlobalProperties.GetIPGlobalProperties();

			// If the ip we want a port on is an 'any' or loopback port we need to exclude all ports that are active on any IP
			Func<IPAddress, bool> IsIpAnyOrLoopBack = (Ip) =>
			{
				return
					IPAddress.Any.Equals(Ip) ||
					IPAddress.IPv6Any.Equals(Ip) ||
					IPAddress.Loopback.Equals(Ip) ||
					IPAddress.IPv6Loopback.
					Equals(Ip)
				;
			};

			// Get all active ports on specified IP. 
			var ExcludedPorts = new List<ushort>();


			// If a port is open on an 'any' or 'loopback' interface then include it in the excludedPorts
			ExcludedPorts.AddRange(
				from n in IpProperties.GetActiveTcpConnections()
				where
					n.LocalEndPoint.Port >= RangeStart
					&& n.LocalEndPoint.Port <= RangeEnd
					&&
					(
						IsIpAnyOrLoopBack(BindIp) ||
						n.LocalEndPoint.Address.Equals(BindIp) ||
						IsIpAnyOrLoopBack(n.LocalEndPoint.Address)
					)
					&& (!IncludeIdlePorts || n.State != TcpState.TimeWait)
				select (ushort)n.LocalEndPoint.Port
			);

			ExcludedPorts.AddRange(
				from n in IpProperties.GetActiveTcpListeners()
				where
					n.Port >= RangeStart && n.Port <= RangeEnd
					&&
					(IsIpAnyOrLoopBack(BindIp) || n.Address.Equals(BindIp) || IsIpAnyOrLoopBack(n.Address))
				select (ushort)n.Port
			);

			ExcludedPorts.AddRange(
				from n in IpProperties.GetActiveUdpListeners()
				where
					n.Port >= RangeStart && n.Port <= RangeEnd
					&&
					(IsIpAnyOrLoopBack(BindIp) || n.Address.Equals(BindIp) || IsIpAnyOrLoopBack(n.Address))
				select (ushort)n.Port
			);

			ExcludedPorts.Sort();


			for (ushort Port = RangeStart; Port <= RangeEnd; Port++)
			{
				if (!ExcludedPorts.Contains(Port))
				{
					return Port;
				}
			}

			return 0;
		}

		public static NetworkInterface[] GetActiveNetworkInterfaces()
		{
			return (NetworkInterface.GetAllNetworkInterfaces().Where(p => p.OperationalStatus == OperationalStatus.Up)).ToArray();
		}
	}
}
