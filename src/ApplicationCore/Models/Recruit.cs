using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using ApplicationCore.Helpers;


namespace ApplicationCore.Models
{
	public class Recruit : BaseCategory
	{
		public int Year { get; set; }

		public DateTime? Date { get; set; }

		public bool Done { get; set; }

		public int SubjectId { get; set; }

		public ICollection<RecruitQuestion> RecruitQuestions { get; set; }

		[NotMapped]
		public ICollection<Recruit> SubItems { get; private set; }


		public void LoadSubItems(IEnumerable<Recruit> subItems)
		{
			SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}
		}

	}

}
