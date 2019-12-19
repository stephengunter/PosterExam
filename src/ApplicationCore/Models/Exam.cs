using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Exam : BaseRecord
	{
		public OptionType OptionType { get; private set; }
		public int OptionCount { get; private set; }

		public double Score { get; set; }
		public string UserId { get; set; }
		public ICollection<ExamQuestion> ExamQuestions { get; set; }

		public User User { get; set; }

		private Exam() { }

		public Exam(int optionCount, OptionType optionType)
		{
			this.OptionCount = optionCount;
			this.OptionType = optionType;
		}
	}

	public enum OptionType
	{
		Number,
		Alphabet
	}
}
