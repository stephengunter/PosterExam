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
	public static class SubjectsViewService
	{
		public static SubjectViewModel MapViewModel(this Subject subject, IMapper mapper)
		{
			return mapper.Map<SubjectViewModel>(subject);
		}

		public static List<SubjectViewModel> MapViewModelList(this IEnumerable<Subject> subjects, IMapper mapper)
		{
			return subjects.Select(item => MapViewModel(item, mapper)).ToList();
		}

		public static Subject MapEntity(this SubjectViewModel model, IMapper mapper, Subject subject = null)
		{
			if (subject == null) subject = new Subject();
			
			return mapper.Map<SubjectViewModel, Subject>(model, subject);
		}

		public static IEnumerable<Subject> GetOrdered(this IEnumerable<Subject> subjects)
		{
			return subjects.OrderBy(item => item.Order);
		}
	}
}
