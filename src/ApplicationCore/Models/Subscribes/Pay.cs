using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models
{
	public class Pay : BaseRecord
	{
		public string Code { get; set; }

		public int BillId { get; set; }

		public decimal Money { get; set; }

		public int PayWayId { get; set; }
		

		public Bill Bill { get; set; }
	}

	public class PayWay : BaseRecord
	{
		public string Code { get; set; }

		public string Title { get; set; }

	}
}
