using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class UrlString
	{
		public string String { get; private set; }

		public string Schema { get; private set; }
		public string User { get; private set; }
		public string Password { get; private set; }
		public string Domain { get; private set; }
		public int? Port { get; private set; }
		public string Path { get; private set; }
		public QueryString Query { get; private set; }

		//public bool HasAuthority { get { return (User != null || Password != null); } }
		//public bool IsAbsolute { get { return Schema != null; } }

		public string Full
		{
			get
			{ 
				var Return = new StringBuilder();
				if (Schema != null)
				{
					Return.Append(Schema).Append("://");
					if (User != null || Password != null) Return.Append(User).Append(":").Append(Password).Append("@");
					if (Domain != null) Return.Append(Domain);
					if (Port != null) Return.Append(":").Append(Port.Value);
				}
				if (Path != null) Return.Append(Path);
				if (Query != null && Query.String != null && Query.String != "") Return.Append("?").Append(Query);
				return Return.ToString();
			}
		}

		private void InternalParse(string String)
		{
			this.String = String;
			this.Schema = null;
			this.User = null;
			this.Password = null;
			this.Domain = null;
			this.Port = null;
			this.Path = null;
			this.Query = new QueryString("");

			// Has schema
			var SchemaStart = String.IndexOf("://");
			if (SchemaStart >= 0)
			{
				this.Schema = String.Substring(0, SchemaStart);
				String = String.Substring(SchemaStart + 3);

				// User/password
				var UserPasswordStart = String.IndexOf('@');
				if (UserPasswordStart != -1)
				{
					var UserPassword = String.Substring(0, UserPasswordStart).Split(':');
					if (UserPassword.Length >= 1) this.User = UserPassword[0];
					if (UserPassword.Length >= 2) this.Password = UserPassword[1];
					String = String.Substring(SchemaStart + 1);
				}

				// Domain
				var EndDomainStart = String.IndexOfAny(new[] { '/', '?', ':' });
				if (EndDomainStart == -1) EndDomainStart = String.Length;
				this.Domain = String.Substring(0, EndDomainStart);
				String = String.Substring(EndDomainStart);

				// Port
				if (String.Length > 0 && String[0] == ':')
				{
					String = String.Substring(1);
					int n = 0;
					for (; n < String.Length; n++)
					{
						if (!(String[n] >= '0' && String[n] <= '9')) break;
					}
					if (n > 0)
					{
						this.Port = int.Parse(String.Substring(0, n));
						String = String.Substring(n);
					}
				}
			}

			// Path
			var QueryStart = String.IndexOf('?');
			if (QueryStart == -1) QueryStart = String.Length;
			this.Path = String.Substring(0, QueryStart);
			String = String.Substring(QueryStart);

			// QueryString
			if (String.Length > 0)
			{
				if (String[0] == '?')
				{
					this.Query = String.Substring(1);
					String = "";
				}
			}
		}

		static public implicit operator UrlString(string String)
		{
			var UrlString = new UrlString();
			UrlString.InternalParse(String);
			return UrlString;
		}

		static public implicit operator string(UrlString UrlString)
		{
			return UrlString.String;
		}

		public override string ToString()
		{
			return Full;
		}
	}
}
