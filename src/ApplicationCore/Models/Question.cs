using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using ApplicationCore.Helpers;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Question : BaseRecord
	{
		public string Title { get; set; }

		public bool MultiAnswers { get; set; }

		public int SubjectId { get; set; }
		public Subject Subject { get; set; }

		public string TermIds { get; set; }  //  Example: 1,6

		public ICollection<Option> Options { get; set; } = new List<Option>();

		public ICollection<Resolve> Resolves { get; set; } = new List<Resolve>();


		public ICollection<RecruitQuestion> RecruitQuestions { get; set; } = new List<RecruitQuestion>();

		[NotMapped]
		public ICollection<Recruit> Recruits
		{
			get
			{
				if (this.RecruitQuestions.IsNullOrEmpty()) return null;
				return this.RecruitQuestions.Select(item => item.Recruit).ToList();
			}
		}


		[NotMapped]
		public ICollection<Term> Terms { get; private set; }

		public void LoadTerms(IEnumerable<Term> allTerms)
		{
			var termIds = TermIds.SplitToIds();
			this.Terms = allTerms.Where(x => termIds.Contains(x.Id)).ToList();
		}
	}
}
