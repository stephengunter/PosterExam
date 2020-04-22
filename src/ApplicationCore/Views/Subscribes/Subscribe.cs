using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Views
{
	public class SubscribeViewModel : BaseRecordView
	{
		public int Id { get; set; }

		public int PlanId { get; set; }

		public int BillId { get; set; }

		public string StartDateText { get; set; }

		public string EndDateText { get; set; }

		public bool Before { get; set; }

		public bool Ended { get; set; }

		public override string StatusText
		{
			get
			{
				if (Active) return "進行中";
				else if(Before) return "未開始";
				else if (Ended) return "已結束";
				return "";
			}
		}

	}
}
