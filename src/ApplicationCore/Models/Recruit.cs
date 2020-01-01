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

		public string PS { get; set; }

		public ICollection<RecruitQuestion> RecruitQuestions { get; set; }

		[NotMapped]
		public Subject Subject { get; set; }

		[NotMapped]
		public ICollection<int> SubjectIds { get; set; } = new List<int>();

		[NotMapped]
		public ICollection<Recruit> SubItems { get; private set; }

		public ICollection<int> GetSubIds()
		{
			var subIds = new List<int>();
			foreach (var item in SubItems)
			{
				subIds.Add(item.Id);

				subIds.AddRange(item.GetSubIds());
			}
			return subIds;
		}


		public void LoadSubItems(IEnumerable<Recruit> subItems)
		{
			SubItems = subItems.Where(item => item.ParentId == this.Id).OrderBy(item => item.Order).ToList();

			foreach (var item in SubItems)
			{
				item.LoadSubItems(subItems);
			}
		}

		public Recruit GetParent(IEnumerable<Recruit> rootItems) => rootItems.FirstOrDefault(x => x.Id == ParentId);



	}

}
