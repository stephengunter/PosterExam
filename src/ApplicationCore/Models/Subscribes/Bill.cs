using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models
{
	public class Bill : BaseRecord
	{
		public string UserId { get; set; }

		public string Code { get; set; } = Guid.NewGuid().ToString("N");

		public int PlanId { get; set; }

		public decimal Amount { get; set; }

		public bool HasDiscount { get; set; }

		public DateTime DeadLine { get; set; }

		public string BankName { get; set; }

		public string BankCode { get; set; }

		public int PayWayId { get; set; }

		public PayWay PayWay { get; set; }

		public Plan Plan { get; set; }

		public User User { get; set; }

		public ICollection<Subscribe> Subscribes { get; set; } = new List<Subscribe>();

		public ICollection<Pay> Pays { get; set; } = new List<Pay>();

		[NotMapped]
		public bool Payed => Pays.IsNullOrEmpty() ? false : Pays.Where(p => !p.Removed).Sum(p => p.Money) >= Amount;

		[NotMapped]
		public DateTime? PayedDate
		{
			get
			{
				if (Payed) return Pays.OrderByDescending(p => p.CreatedAt).FirstOrDefault().CreatedAt;
				return null;
			}
		}
	}
}
