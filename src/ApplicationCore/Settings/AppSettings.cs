using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Settings
{
	public class AppSettings
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public string ClientUrl { get; set; }
		public string AdminUrl { get; set; }
		public string BackendUrl { get; set; }
		public string PayUrl { get; set; }

		public string UploadPath { get; set; }
	}

	public class RootSubjectSettings
	{
		//專業科目(1)：臺灣自然及人文地理
		public int FirstId { get; set; }

		//專業科目(2)：郵政法規大意及交通安全常識
		public int SecondId { get; set; }
	}

	public class AuthSettings
	{
		public string SecurityKey { get; set; }
		public int TokenValidHours { get; set; }
		public int RefreshTokenDaysToExpire { get; set; }

	}

	public class SubscribesSettings
	{
		public int BillDaysToExpire { get; set; }

	}

	public class AdminSettings
	{
		public string Url { get; set; }
		public string Key { get; set; }
		public string BackupPath { get; set; }
		public string DataPath { get; set; }
	}
}
