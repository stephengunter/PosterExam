using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;

namespace ApplicationCore.ViewServices
{
	public static class TermsViewService
	{
		public static TermViewModel MapViewModel(this Term term, IMapper mapper)
		{
			return mapper.Map<TermViewModel>(term);
		}

		public static List<TermViewModel> MapViewModelList(this IEnumerable<Term> terms, IMapper mapper)
		{
			return terms.Select(item => MapViewModel(item, mapper)).ToList();
		}

		public static Term MapEntity(this TermViewModel model, IMapper mapper, Term term = null)
		{
			if (term == null) term = new Term();
			
			return mapper.Map<TermViewModel, Term>(model, term);
		}
	}
}
