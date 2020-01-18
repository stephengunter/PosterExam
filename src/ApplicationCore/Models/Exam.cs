using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models
{
	public class Exam : BaseRecord
	{
		public ExamType ExamType { get; set; }
		public RecruitExamType RecruitExamType { get; set; } = RecruitExamType.Unknown;
		public OptionType OptionType { get; set; }

		public int Year { get; set; }
		public int SubjectId { get; set; }
		

		public double Score { get; set; }
		public bool Reserved { get; set; }
		public string UserId { get; set; }

		public ICollection<ExamPart> Parts { get; set; } = new List<ExamPart>();

		public User User { get; set; }
		
	}


	public class ExamPart : BaseEntity
	{ 
		public int ExamId { get; set; }
		public string Title { get; set; }
		public int OptionCount { get; set; }
		public double Points { get; set; }
		public bool MultiAnswers { get; set; }
		public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
		public Exam Exam { get; set; }
	}

	public enum ExamType
	{
		Recruit = 0, //歷屆試題
		System = 1, //系統自訂
		Unknown = -1
	}

	public enum RecruitExamType //歷屆試題測驗模式
	{
		Exactly = 0,  //完全相同
		CrossYears = 1,  //各年度交叉
		Unknown = -1
	}

	public enum OptionType
	{
		Number = 0,   // 1,2,3,4
		Alphabet = 1, // A,B,C,D
		Unknown = -1
	}
}
