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
	public class SubscribesController : BaseApiController
	{
		private readonly ISubscribesService _subscribesService;
		private readonly IPlansService _plansService;
		private readonly IPayWaysService _paywaysService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;

		
		public SubscribesController(ISubscribesService subscribesService, IPlansService plansService,
			IPayWaysService paywaysService, IAppLogger logger, IMapper mapper)
		{
			_subscribesService = subscribesService;
			_plansService = plansService;
			_paywaysService = paywaysService;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var model = await GetIndexViewAsync();
			
			var payways = (await _paywaysService.FetchAsync()).GetOrdered();
			model.PayWays = payways.MapViewModelList(_mapper);

			return Ok(model);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] SubscribeEditForm form)
		{
			if (form.Plan == null)
			{
				ModelState.AddModelError("plan", "無法讀取訂單資料");
			}
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var model = await GetIndexViewAsync();
			var activePlan = CheckForExceptions(model, form);

			//開始建立Bill
			var bill = new Bill
			{
				UserId = CurrentUserId,
				PlanId = activePlan.Id,
				Amount = activePlan.Price,
				HasDiscount = activePlan.HasDiscount,
				//PayWay = 
			};



			return Ok();
		}

		async Task<SubscribesIndexViewModel> GetIndexViewAsync()
		{
		
			var subscribes = await _subscribesService.FetchByUserAsync(CurrentUserId);

			var model = new SubscribesIndexViewModel();

			if (subscribes.HasItems())
			{
				//User訂閱紀錄
				model.Records = subscribes.MapViewModelList(_mapper);


				//訂閱期之內(Active)的訂閱紀錄
				var activeSubscribe = subscribes.Where(item => item.Active).FirstOrDefault();
				if (activeSubscribe != null) model.Current = activeSubscribe.MapViewModel(_mapper);
			}

			if (model.Current == null)
			{
				//目前不在訂閱期
				var plan = await FindPlanAsync();
				if (plan != null)
				{
					bool canDiscount = subscribes.Where(x => x.Payed).HasItems();
					model.Plan = plan.MapViewModel(_mapper, canDiscount);
				}
			}

			return model;
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



		PlanViewModel CheckForExceptions(SubscribesIndexViewModel model, SubscribeEditForm form)
		{
			if (model.Current != null)
			{
				//目前仍在訂閱期
				throw new CreateWhileCurrentSubscribeExist(CurrentUserId);

			}

			if (model.Plan == null) throw new Exception();

			var selectedPlan = form.Plan;
			var activePlan = model.Plan;

			if (selectedPlan.Id != activePlan.Id)
			{
				//找不到指定的方案
				throw new SelectedPlanDifferentFromActivePlan(selectedPlan.Id, activePlan.Id);
			}

			if (selectedPlan.Price != activePlan.Price)
			{
				//找不到指定的方案
				throw new SelectedPriceDifferentFromActivePlan(selectedPlan.Id, activePlan.Id);
			}

			return activePlan;
		}

	}

	
}
