using ApplicationCore.Helpers;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Models
{
	public class Pay : BaseRecord
	{
		public static Pay Create(Bill bill, PayWay payWay, ThirdPartyPayment thirdPartyPayment)
		{
			return new Pay
			{
				BillId = bill.Id,
				PayWay = payWay.Code,
				Provider = thirdPartyPayment.ToString()
			};

		}


		public int BillId { get; set; }
		
		public string Code { get; set; } = TickId.Create(20);

		public string PayWay { get; set; }

		public string TradeNo { get; set; }

		public string BankCode { get; set; }

		public string BankAccount { get; set; }

		public string Provider { get; set; }

		public decimal Money { get; set; }

		public DateTime? PayedDate { get; set; }

		public string TradeData { get; set; } //json string


		public Bill Bill { get; set; }


		[NotMapped]
		public bool HasMoney => Removed ? false : Money > 0;
	}

	public class PayWay : BaseRecord
	{
		public string Code { get; set; }

		public string Title { get; set; }

	}
}
