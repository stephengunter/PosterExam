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
	public static class PaysViewService
	{
		public static PayViewModel MapViewModel(this Pay pay, IMapper mapper, IEnumerable<PayWay> payWays = null)
		{
			var model = mapper.Map<PayViewModel>(pay);

			if (payWays.HasItems())
			{
				var payway = payWays.FirstOrDefault(x => x.Code == pay.PayWay);
				if(payway != null) model.PayWayTitle = payway.Title;
			}

			return model;
		}

		public static PayWayViewModel MapViewModel(this PayWay payWay, IMapper mapper)
		{ 
		    var model = mapper.Map<PayWayViewModel>(payWay);
			
			return model;
		}

		

		public static List<PayViewModel> MapViewModelList(this IEnumerable<Pay> pays, IMapper mapper, IEnumerable<PayWay> payWays = null)
			=> pays.Select(item => MapViewModel(item, mapper, payWays)).ToList();

		public static List<PayWayViewModel> MapViewModelList(this IEnumerable<PayWay> payWays, IMapper mapper)
			=> payWays.Select(item => MapViewModel(item, mapper)).ToList();

		

		public static PayWay MapEntity(this PayWayViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<PayWayViewModel, PayWay>(model);
			
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<PayWay> GetOrdered(this IEnumerable<PayWay> payWays)
		{
			return payWays.OrderBy(item => item.Order);

		}

	}
}
