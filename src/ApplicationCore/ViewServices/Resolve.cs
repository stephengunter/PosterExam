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
using Newtonsoft.Json;

namespace ApplicationCore.ViewServices
{
	public static class ResolveViewService
	{
		public static ResolveViewModel MapViewModel(this Resolve option, IMapper mapper)
		{
			var model = mapper.Map<ResolveViewModel>(option);
			model.Highlights = JsonConvert.DeserializeObject<ICollection<string>>(model.Highlight);
			
			
			return model;
		}

		public static List<ResolveViewModel> MapViewModelList(this IEnumerable<Resolve> resolves, IMapper mapper)
			=> resolves.Select(item => MapViewModel(item, mapper)).ToList();

		public static Resolve MapEntity(this ResolveViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<ResolveViewModel, Resolve>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			entity.Text = entity.Text.ReplaceNewLine();

			entity.Highlight = model.Highlights.HasItems() ? JsonConvert.SerializeObject(model.Highlights) : "";



			return entity;
		}

		public static PagedList<Resolve, ResolveViewModel> GetPagedList(this IEnumerable<Resolve> resolves, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Resolve, ResolveViewModel>(resolves, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}
	}
}
