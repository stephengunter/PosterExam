using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;

namespace ApplicationCore.Models
{
	public class Plan : BaseRecord, IBaseContract
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public decimal Money { get; set; }

		public int Discount { get; set; }

		public ICollection<Subscribe> Subscribes { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public override bool Active
		{
			get
			{
				if (!StartDate.HasValue) return false;

				DateTime endDate = DateTime.MaxValue;
				if (this.EndDate.HasValue) endDate = Convert.ToDateTime(this.EndDate);

				return (DateTime.Now >= StartDate.Value) && DateTime.Now <= endDate;
			}
		}

		public bool Before
		{
			get
			{
				if (!StartDate.HasValue) return true;

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

		public bool HasDateConflict(Plan model)
		{
			if(!this.StartDate.HasValue) return false;
			if(!model.StartDate.HasValue) return false;

			DateTime startDate = Convert.ToDateTime(this.StartDate);
			DateTime inputStartDate = Convert.ToDateTime(model.StartDate);

			DateTime endDate = DateTime.MaxValue;
			if (this.EndDate.HasValue) endDate = Convert.ToDateTime(this.EndDate);

			DateTime inputEndDate = DateTime.MaxValue;
			if (model.EndDate.HasValue) inputEndDate = Convert.ToDateTime(model.EndDate);

			if (inputStartDate >= startDate)
			{
				return inputStartDate <= endDate;
			}
			else
			{
				return inputEndDate >= startDate;
			}

		}
	}
}
