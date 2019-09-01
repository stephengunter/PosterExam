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
		}
	}
}
