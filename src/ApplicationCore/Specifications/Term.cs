using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class TermFilterSpecification : BaseSpecification<Term>
	{
		public TermFilterSpecification() : base(item => !item.Removed) { }

		public TermFilterSpecification(Subject subject) : base(item => !item.Removed && item.SubjectId == subject.Id) { }

		public TermFilterSpecification(Subject subject, int parentId)
			: base(item => !item.Removed && item.SubjectId == subject.Id && item.ParentId == parentId) { }
	}
}
