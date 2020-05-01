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
using ApplicationCore.Logging;
using ApplicationCore.Exceptions;

namespace Web.Controllers.Api
{
	public class PlansController : BaseApiController
	{
		private readonly IPlansService _plansService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;

		
		public PlansController(IPlansService plansService,
			IAppLogger logger, IMapper mapper)
		{
			
			_plansService = plansService;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var plan = await FindPlanAsync();
			if (plan == null) return NotFound();
			
			return Ok(plan.MapViewModel(_mapper));
			
		}

		async Task<Plan> FindPlanAsync()
		{
			bool active = true;
			var activePlans = await _plansService.FetchAsync(active);

			if (activePlans.IsNullOrEmpty())
			{
				//例外
				_logger.LogException(new NoActivePlanFound());

				return null;
			}
			else if (activePlans.Count() > 1)
			{
				//例外
				_logger.LogException(new MutiActivePlanFound());

				return activePlans.GetOrdered().FirstOrDefault();
			}
			else return activePlans.FirstOrDefault();
		}

	}

	
}
