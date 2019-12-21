using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Views
{
	public abstract class BaseRecordView
	{
		public DateTime CreatedAt { get; set; }
		public DateTime LastUpdated { get; set; }
		public string UpdatedBy { get; set; }

		public int Order { get; set; }
		public bool Removed { get; set; }

		public bool Active { get; set; }

		public string StatusText => this.Active ? "上架中" : "已下架";

		public void SetUpdated(string userId)
		{
			UpdatedBy = userId;
			LastUpdated = DateTime.Now;
		}

	}
	
}
