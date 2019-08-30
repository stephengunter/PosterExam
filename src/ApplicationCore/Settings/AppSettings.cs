using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Settings
{
	public class AppSettings
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }

	}

	public class AuthSettings
	{
		public string SecurityKey { get; set; }
		public int RefreshTokenDaysToExpire { get; set; }		

	}

	public class AdminSettings
	{
		public string Url { get; set; }
		public string Key { get; set; }
	}
}
