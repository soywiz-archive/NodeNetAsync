#define DISABLE_PROPAGATING

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
#if DISABLE_PROPAGATING
			return await Task.Run(() =>
			{
				return Action();
			});
#else
			Exception YieldedException = null;

			//Console.WriteLine(Environment.StackTrace);

			var Result = await Task.Run(() =>
			{
				try
				{
					return Action();
				}
				catch (Exception Exception)
				//catch (Exception)
				{
					YieldedException = Exception;
					return default(TReturn);
				}
			});

			if (YieldedException != null) throw YieldedException;

			return Result;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TReturn"></typeparam>
		/// <param name="Action"></param>
		/// <returns></returns>
		async static public Task RunPropagatingExceptionsAsync(Action Action)
		{
#if DISABLE_PROPAGATING
			await Task.Run(() =>
			{
				Action();
			});
#else
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
#endif
		}
	}
}
