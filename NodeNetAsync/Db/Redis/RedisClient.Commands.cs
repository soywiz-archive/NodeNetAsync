using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Redis
{
    public partial class RedisClient
    {
		/// <summary>
		/// Append a value to a key
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="Value"></param>
		/// <remarks>
		/// O(1). The amortized time complexity is O(1) assuming the appended value is
		/// small and the already present value is of any size, since the dynamic string
		/// library used by Redis will double the free space available on every reallocation.
		/// 
		/// If key already exists and is a string, this command appends the value at the
		/// end of the string. If key does not exist it is created and set as an empty string,
		/// so APPEND will be similar to SET in this special case.
		/// </remarks>
		/// <returns>The length of the string after the append operation.</returns>
		/// <see cref="http://redis.io/commands/append"/>
		async public Task<int> AppendAsync(string Key, string Value)
		{
			return (int)(await CommandAsync("set", Key, Value));
		}

		/// <summary>
		/// Authenticate to the server
		/// </summary>
		/// <remarks>
		/// Request for authentication in a password protected Redis server. Redis can be instructed
		/// to require a password before allowing clients to execute commands. This is done using the
		/// requirepass directive in the configuration file.
		/// 
		/// If password matches the password in the configuration file, the server replies with the OK
		/// status code and starts accepting commands. Otherwise, an error is returned and the clients
		/// needs to try a new password.
		/// 
		/// Note: because of the high performance nature of Redis, it is possible to try a lot of
		/// passwords in parallel in very short time, so make sure to generate a strong and very
		/// long password so that this attack is infeasible.
		/// </remarks>
		/// <param name="Password"></param>
		/// <returns></returns>
		/// <see cref="http://redis.io/commands/auth"/>
		async public Task<object> AuthAsync(string Password)
		{
			return await CommandAsync("auth", Password);
		}

		/// <summary>
		/// Asynchronously rewrite the append-only file
		/// </summary>
		/// <remarks>
		/// Rewrites the append-only file to reflect the current dataset in memory.
		/// If BGREWRITEAOF fails, no data gets lost as the old AOF will be untouched.
		/// </remarks>
		/// <param name="Key"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		/// <see cref="http://redis.io/commands/bgrewriteaof"/>
		async public Task BackgroundRewriteAofAsync()
		{
			await CommandAsync("bgrewriteaof");
		}

		/// <summary>
		/// Asynchronously save the dataset to disk
		/// </summary>
		/// <remarks>
		/// Save the DB in background. The OK code is immediately returned. Redis forks,
		/// the parent continues to server the clients, the child saves the DB on disk then exit.
		/// A client my be able to check if the operation succeeded using the LASTSAVE command.
		/// </remarks>
		/// <returns></returns>
		/// <see cref="http://redis.io/commands/bgsave"/>
		async public Task BackgroundSave()
		{
			await CommandAsync("bgrewriteaof");
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
    }
}
