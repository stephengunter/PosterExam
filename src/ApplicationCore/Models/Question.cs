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



		public string RecruitsText => Recruits.IsNullOrEmpty() ? "" : String.Join(",", Recruits.Select(item => item.Title));
	}
}
