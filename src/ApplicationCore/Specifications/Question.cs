using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class QuestionFilterSpecification : BaseSpecification<Question>
	{
		public QuestionFilterSpecification(int subjectId) : base(item => !item.Removed && item.SubjectId == subjectId)
		{
			AddInclude(item => item.Options);
			AddInclude("RecruitQuestions.Recruit");
		}
	}

	public class QuestionIdFilterSpecification : BaseSpecification<Question>
	{
		public QuestionIdFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
		{
			AddInclude(item => item.Options);
			AddInclude("RecruitQuestions.Recruit");
		}
	}
}
