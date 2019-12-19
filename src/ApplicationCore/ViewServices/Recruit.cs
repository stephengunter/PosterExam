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
	public static class RecruitsViewService
	{
		public static RecruitViewModel MapViewModel(this Recruit recruit, IMapper mapper) => mapper.Map<RecruitViewModel>(recruit);

		public static List<RecruitViewModel> MapViewModelList(this IEnumerable<Recruit> recruits, IMapper mapper) => recruits.Select(item => MapViewModel(item, mapper)).ToList();

		public static Recruit MapEntity(this RecruitViewModel model, IMapper mapper) => mapper.Map<RecruitViewModel, Recruit>(model);

		public static IEnumerable<Recruit> GetOrdered(this IEnumerable<Recruit> recruits) => recruits.OrderBy(item => item.Order);
	}
}
