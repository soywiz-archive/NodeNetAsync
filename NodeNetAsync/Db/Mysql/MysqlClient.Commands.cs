using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Db.Mysql
{
	public partial class MysqlClient
	{
		AsyncTaskQueue AsyncTaskQueue = new AsyncTaskQueue();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		async public Task PingAsync()
		{
			await AsyncTaskQueue.EnqueueAsync(async () =>
			{
				var OutPacket = new MysqlPacket(ConnectionEncoding, 0);
				OutPacket.WriteNumber(1, (uint)MysqlCommandEnum.COM_PING);
				await SendPacketAsync(OutPacket);
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="DatabaseName"></param>
		/// <returns></returns>
		async public Task SelectDatabaseAsync(string DatabaseName)
		{
			await AsyncTaskQueue.EnqueueAsync(async () =>
			{
				var OutPacket = new MysqlPacket(ConnectionEncoding, 0);
				OutPacket.WriteNumber(1, (uint)MysqlCommandEnum.COM_INIT_DB);
				OutPacket.WryteBytes(ConnectionEncoding.GetBytes(DatabaseName));
				await SendPacketAsync(OutPacket);

				await ReadPacketAsync();
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Query"></param>
		/// <returns></returns>
		async public Task<MysqlQueryResult> QueryAsync(string Query, params object[] Params)
		{
			var MysqlQueryResult = new MysqlQueryResult();

			Query = ReplaceParameters(Query, Params);

			await AsyncTaskQueue.EnqueueAsync(async () =>
			{
				var OutPacket = new MysqlPacket(ConnectionEncoding, 0);
				OutPacket.WriteNumber(1, (uint)MysqlCommandEnum.COM_QUERY);
				OutPacket.WryteBytes(ConnectionEncoding.GetBytes(Query));
				await SendPacketAsync(OutPacket);

				int NumberOfFields = HandleResultSetHeaderPacket(await ReadPacketAsync());
				//Console.WriteLine("Number of fields: {0}", NumberOfFields);

				// Read fields
				while (true)
				{
					var InPacket = await ReadPacketAsync();
					if (CheckEofPacket(InPacket)) break;
					MysqlQueryResult.Columns.Add(HandleFieldPacket(InPacket));
				}

				// Read words
				while (true)
				{
					var InPacket = await ReadPacketAsync();
					if (CheckEofPacket(InPacket)) break;
					MysqlQueryResult.Rows.Add(HandleRowDataPacket(InPacket, MysqlQueryResult.Columns));
				}
			});

			return MysqlQueryResult;
		}
	}
}
