using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System;
using System.Collections.Generic;

namespace ApplicationCore.Specifications
{

	public class RecruitFilterSpecification : BaseSpecification<Recruit>
	{
		public RecruitFilterSpecification() : base(item => !item.Removed)
		{

		}

		public RecruitFilterSpecification(bool active) : base(item => !item.Removed && item.Active)
		{

		}
	}
}
