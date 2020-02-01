using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Specifications
{

	public class NoteFilterSpecification : BaseSpecification<Note>
	{
		public NoteFilterSpecification() : base(item => !item.Removed) 
		{
			
		}

		public NoteFilterSpecification(Term term) : base(item => !item.Removed && item.TermId == term.Id)
		{

		}

	}

	public class NoteTermFilterSpecification : BaseSpecification<Note>
	{
		public NoteTermFilterSpecification(IList<int> termIds) : base(item => !item.Removed && termIds.Contains(item.TermId))
		{

		}

		

		public NoteTermFilterSpecification(Term term, int parentId)
			: base(item => !item.Removed && item.TermId == term.Id && item.ParentId == parentId)
		{

		}

	}
}
