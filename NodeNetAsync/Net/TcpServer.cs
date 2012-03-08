using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net
{
	public class TcpServer
	{
		TcpListener TcpListener;
		public event Func<TcpSocket, Task> HandleClient;
		protected bool CatchExceptions;

		public TcpServer(ushort Port, string Bind = "127.0.0.1", bool CatchExceptions = true)
		{
			this.TcpListener = new TcpListener(IPAddress.Parse(Bind), Port);
			this.CatchExceptions = CatchExceptions;
		}

		async private Task HandleClientInternal(TcpSocket Socket)
		{
			if (HandleClient != null)
			{
				if (CatchExceptions)
				{
					Exception YieldedException = null;

					try
					{
						await HandleClient(Socket);
					}
					catch (IOException)
					{
					}
					catch (Exception CatchedException)
					{
						YieldedException = CatchedException;
					}

					if (YieldedException != null)
					{
						await Console.Error.WriteLineAsync(String.Format("{0}", YieldedException));
					}
				}
				else
				{
					await HandleClient(Socket);
				}
			}
		}

		async public Task ListenAsync(int Times = -1)
		{
			Console.Write(String.Format("Starting socket at {0}...", TcpListener.LocalEndpoint));
			TcpListener.Start();
			//await Console.Out.WriteLineAsync(String.Format("Started socket at {0}", TcpListener.LocalEndpoint));
			Console.WriteLine(String.Format("Ok"));

			Task LastClient = null;

			while (Times != 0)
			{
				LastClient = HandleClientInternal(new TcpSocket(await TcpListener.AcceptTcpClientAsync()));
				if (Times > 0) Times--;
			}

			if (LastClient != null) await LastClient;

			Console.WriteLine("Done");
		}

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
