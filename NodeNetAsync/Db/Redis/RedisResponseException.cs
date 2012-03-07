using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Db.Redis
{
	public class RedisResponseException : Exception
	{
		public RedisResponseException(String Message) : base(Message) { }
	}
}
