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
using Web.Helpers;
using Web.Controllers;
using Web.Models;
using ApplicationCore.Logging;
using ApplicationCore.Exceptions;

namespace Web.Controllers.Api
{
	[Authorize]
	public class BillsController : BaseApiController
	{
		private readonly IBillsService _billsService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;

		
		public BillsController(IBillsService billsService,
			IAppLogger logger, IMapper mapper)
		{
			
			_billsService = billsService;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var bills = await _billsService.FetchByUserAsync(CurrentUserId);
			
			
			return Ok(bills.MapViewModelList(_mapper));
			
		}

	}

	
}
