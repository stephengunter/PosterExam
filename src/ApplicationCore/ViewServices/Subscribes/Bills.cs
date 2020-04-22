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
	public static class BillsViewService
	{

		public static BillViewModel MapViewModel(this Bill bill, IMapper mapper)
		{ 
		    var model = mapper.Map<BillViewModel>(bill);
			
			return model;
		}

		public static List<BillViewModel> MapViewModelList(this IEnumerable<Bill> bills, IMapper mapper)
			=> bills.Select(item => MapViewModel(item, mapper)).ToList();

		
		public static Bill MapEntity(this BillViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<BillViewModel, Bill>(model);
			
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}

		public static IEnumerable<Bill> GetOrdered(this IEnumerable<Bill> bills)
		{
			return bills.OrderByDescending(item => item.CreatedAt);

		}

	}
}
