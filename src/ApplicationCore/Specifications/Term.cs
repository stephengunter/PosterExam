using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class TermFilterSpecification : BaseSpecification<Term>
	{
		public TermFilterSpecification(int subjectId, int parentId) : base(item => !item.Removed && item.SubjectId == subjectId && item.ParentId == parentId)
		{

		}
	}
}
