using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	static public class TaskEx
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="Action"></param>
		/// <returns></returns>
		async static public Task<TReturn> RunPropagatingExceptionsAsync<TReturn>(Func<TReturn> Action)
		{
			Exception YieldedException = null;

			var Result = await Task.Run(() =>
			{
				try
				{
					return Action();
				}
				catch (Exception Exception)
				{
					YieldedException = Exception;
					return default(TReturn);
				}
			});

			if (YieldedException != null) throw YieldedException;

			return Result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="Action"></param>
		/// <returns></returns>
		async static public Task RunPropagatingExceptionsAsync<TReturn>(Action Action)
		{
			Exception YieldedException = null;

			await Task.Run(() =>
			{
				try
				{
					Action();
				}
				catch (Exception Exception)
				{
					YieldedException = Exception;
				}
			});

			if (YieldedException != null) throw YieldedException;
		}
	}
}
