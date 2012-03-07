using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Db.Redis
{
	/// <summary>
	/// Simple non-binary-safe Redis Async Client.
	/// </summary>
	/// <see cref="http://redis.io/topics/protocol"/>
	public class RedisClient
	{
		protected TcpSocket TcpClient;
		protected Encoding Encoding;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Host"></param>
		/// <param name="Port"></param>
		/// <param name="Encoding"></param>
		/// <returns></returns>
		async static public Task<RedisClient> CreateAndConnectAsync(string Host, ushort Port = 6379, Encoding Encoding = null)
		{
			var RedisClient = new RedisClient(Encoding);
			await RedisClient.ConnectAsync(Host, Port);
			return RedisClient;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Encoding"></param>
		protected RedisClient(Encoding Encoding = null)
		{
			if (Encoding == null) Encoding = Encoding.UTF8;
			this.Encoding = Encoding;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Host"></param>
		/// <param name="Port"></param>
		/// <returns></returns>
		async protected Task ConnectAsync(string Host, ushort Port = 6379)
		{
			this.TcpClient = await TcpSocket.CreateAndConnectAsync(Host, Port);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		async protected Task<object> ReadValueAsync()
		{
			var FirstLine = await TcpClient.ReadLineAsync(Encoding);

			switch (FirstLine[0])
			{
				// Status reply
				case '+': return FirstLine.Substring(1);
				// Error reply
				case '-': throw (new RedisResponseException(FirstLine.Substring(1)));
				// Integer reply
				case ':': return Convert.ToInt64(FirstLine.Substring(1));
				// Bulk replies
				case '$':
					var BytesToRead = Convert.ToInt32(FirstLine.Substring(1));
					if (BytesToRead == -1) return null;
					var Data = await TcpClient.ReadBytesAsync(BytesToRead);
					await TcpClient.SkipBytesAsync(2);
					return Encoding.GetString(Data);
				case '*':
					var BulksToRead = Convert.ToInt64(FirstLine.Substring(1));
					var Bulks = new object[BulksToRead];
					for (int n = 0; n < BulksToRead; n++)
					{
						Bulks[n] = await ReadValueAsync();
					}
					return Bulks;

				default:
					throw (new RedisResponseException("Unknown param type '" + FirstLine[0] + "'"));
			}
		}

		/// <summary>
		/// Sends a command to the redis server
		/// </summary>
		/// <param name="Arguments">Command parameters</param>
		/// <returns>The command result</returns>
		/// <see cref="http://redis.io/commands"/>
		async public Task<object> CommandAsync(params string[] Arguments)
		{
			var MemoryStream = new MemoryStream();

			var Command = "*" + Arguments.Length + "\r\n";
			foreach (var Argument in Arguments)
			{
				// Length of the argument.
				Command += "$" + Encoding.GetByteCount(Argument) + "\r\n";
				Command += Argument + "\r\n";
			}

			var Data = Encoding.GetBytes(Command);
			await TcpClient.WriteAsync(Data, 0, (int)Data.Length);

			return await ReadValueAsync();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		/// <see cref="http://redis.io/commands/set"/>
		async public Task SetAsync(string Key, string Value)
		{
			await CommandAsync("set", Key, Value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Key"></param>
		/// <returns></returns>
		/// <see cref="http://redis.io/commands/get"/>
		async public Task<object> GetAsync(string Key)
		{
			return await CommandAsync("get", Key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Password"></param>
		/// <returns></returns>
		/// <see cref="http://redis.io/commands/auth"/>
		async public Task<object> AuthAsync(string Password)
		{
			return await CommandAsync("auth", Password);
		}
	}
}
