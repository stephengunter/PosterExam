using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class SubjectFilterSpecification : BaseSpecification<Subject>
	{
		public SubjectFilterSpecification(int parentId) : base(item => !item.Removed && item.ParentId == parentId)
		{

		}
	}
}
