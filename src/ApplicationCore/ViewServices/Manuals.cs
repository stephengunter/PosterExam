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
	public static class ManualsViewService
	{
		public static ManualViewModel MapViewModel(this Manual manual, IMapper mapper, ICollection<UploadFile> attachmentsList = null)
		{
			if (attachmentsList.HasItems()) manual.LoadAttachments(attachmentsList);

			var model = mapper.Map<ManualViewModel>(manual);
			return model;
		}

		public static List<ManualViewModel> MapViewModelList(this IEnumerable<Manual> manuals, IMapper mapper, ICollection<UploadFile> attachmentsList = null)
			=> manuals.Select(item => MapViewModel(item, mapper, attachmentsList)).ToList();

		public static Manual MapEntity(this ManualViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<ManualViewModel, Manual>(model);
			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			entity.Content = entity.Content.ReplaceNewLine();
			return entity;
		}

		public static IEnumerable<Manual> GetOrdered(this IEnumerable<Manual> manuals)
			=> manuals.OrderBy(item => item.Order);
		
	}
}
