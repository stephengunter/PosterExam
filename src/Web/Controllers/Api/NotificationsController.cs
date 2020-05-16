using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api
{
	[Authorize]
	public class NotificationsController : BaseApiController
	{
		private readonly INoticesService _noticesService;
		private readonly IMapper _mapper;

		public NotificationsController(INoticesService noticesService, IMapper mapper)
		{
			_noticesService = noticesService;
			_mapper = mapper;
		}
	

		[HttpGet("")]
		public async Task<ActionResult> Index(int page = 1, int pageSize = 10)
		{
			var notifications = await _noticesService.FetchUserNotificationsAsync(CurrentUserId);

			if (page < 1)
			{
				// 只要未讀的

				notifications = notifications.Where(item => !item.HasReceived);
				if (notifications.IsNullOrEmpty()) return Ok(new List<ReceiverViewModel>());

				notifications = notifications.GetOrdered();
				return Ok(notifications.MapViewModelList(_mapper));

			}

			notifications = notifications.GetOrdered();

			return Ok(notifications.GetPagedList(_mapper, page, pageSize));
		}

	}

	
}
