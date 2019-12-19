using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Recruit : BaseRecord
	{
		public int Year { get; set; }

		public string Title { get; set; }

		public ICollection<RecruitQuestion> RecruitQuestions { get; set; }
	}

	public class RecruitQuestion : IAggregateRoot
	{
		public int RecruitId { get; set; }
		public Recruit Recruit { get; set; }

		public int QuestionId { get; set; }
		public Question Question { get; set; }
	}
}
