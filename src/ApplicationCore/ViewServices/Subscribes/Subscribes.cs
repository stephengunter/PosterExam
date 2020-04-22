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
	public static class SubscribesViewService
	{

		public static SubscribeViewModel MapViewModel(this Subscribe subscribe, IMapper mapper)
		{ 
		    var model = mapper.Map<SubscribeViewModel>(subscribe);
			
			model.StartDateText = subscribe.StartDate.ToDateString();
			model.EndDateText = subscribe.EndDate.ToDateString();
			return model;
		}

		public static List<SubscribeViewModel> MapViewModelList(this IEnumerable<Subscribe> subscribes, IMapper mapper)
			=> subscribes.Select(item => MapViewModel(item, mapper)).ToList();

		
		public static Subscribe MapEntity(this SubscribeViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<SubscribeViewModel, Subscribe>(model);
			
			entity.StartDate = model.StartDateText.ToStartDate();
			entity.EndDate = model.EndDateText.ToStartDate();

			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<Subscribe> GetOrdered(this IEnumerable<Subscribe> subscribes)
		{
			return subscribes.OrderByDescending(item => item.StartDate);

		}

	}
}
