using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;

namespace ApplicationCore.Models
{
	public class Subscribe : BaseRecord, IBaseContract
	{
		public string UserId { get; set; }

		public int BillId { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }


		public User User { get; set; }
		

		public Bill Bill { get; set; }

		public static Subscribe Create(Bill bill)
		{
			var plan = bill.Plan;
			
			return new Subscribe
			{
				BillId = bill.Id,
				UserId = bill.UserId,
				StartDate = DateTime.Now > plan.StartDate ? DateTime.Now : plan.StartDate,
				EndDate = plan.EndDate
			};
		}

		public void OnPayed(Subscribe activeSubscribe = null)
		{
			DateTime dateStart = DateTime.Now;
			if (activeSubscribe != null) dateStart = activeSubscribe.EndDate.Value.AddDays(1);

			StartDate = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 1);
		}


		public bool Payed => StartDate.HasValue;

		public override bool Active
		{
			get
			{
				if (!Payed) return false;
				if (!EndDate.HasValue) return false;

				return (DateTime.Now >= StartDate.Value) && DateTime.Now <= EndDate.Value;
			}

		}

		public bool Before
		{
			get
			{
				if (!Payed) return false;

				return DateTime.Now < StartDate.Value;
			}

		}

		public bool Ended
		{
			get
			{
				if (!EndDate.HasValue) return false;

				return DateTime.Now > EndDate.Value;
			}

		}
	}
}
