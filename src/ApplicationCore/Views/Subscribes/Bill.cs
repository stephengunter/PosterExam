using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApplicationCore.Views
{
    public class BillViewModel : BaseRecordView
    {
		public int Id { get; set; }

		public string UserId { get; set; }

		public string Code { get; set; }

		public int PlanId { get; set; }

		public decimal Amount { get; set; }

		public bool HasDiscount { get; set; }

		public int PayWayId { get; set; }

		public DateTime DeadLine { get; set; }

		public bool Payed { get; set; }

		public decimal NeedPayMoney { get; set; }

		public DateTime? PayedDate { get; set; }
	}
}
