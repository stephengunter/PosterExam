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
	public static class ResolveViewService
	{
		public static ResolveViewModel MapViewModel(this Resolve option, IMapper mapper)
			=> mapper.Map<ResolveViewModel>(option);

		public static Resolve MapEntity(this ResolveViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<ResolveViewModel, Resolve>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}
	}
}
