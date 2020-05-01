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
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;

namespace Web.Controllers.Api
{
	[Authorize]
	public class SubscribesController : BaseApiController
	{
		private readonly SubscribesSettings _subscribesSettings;

		private readonly ISubscribesService _subscribesService;
		private readonly IPlansService _plansService;
		private readonly IPaysService _paysService;
		private readonly IBillsService _billsService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;

		
		public SubscribesController(IOptions<SubscribesSettings> subscribesSettings, 
			ISubscribesService subscribesService, IPlansService plansService,
			IPaysService paysService, IBillsService billsService, 
			IAppLogger logger, IMapper mapper)
		{
			_subscribesSettings = subscribesSettings.Value;

			_subscribesService = subscribesService;
			_plansService = plansService;
			_paysService = paysService;
			_billsService = billsService;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var model = await GetIndexViewAsync();

			var bills = await _billsService.FetchByUserAsync(CurrentUserId);
			if (bills.HasItems()) bills = bills.OrderByDescending(x => x.CreatedAt);

			model.Bills = bills.MapViewModelList(_mapper);

			var payways = (await _paysService.FetchPayWaysAsync()).GetOrdered();
			model.PayWays = payways.MapViewModelList(_mapper);

			return Ok(model);
		}

		[HttpGet("create")]
		public async Task<ActionResult> Create()
		{
			var indexModel = await GetIndexViewAsync();

			var model = new BillEditForm();

			var plan = indexModel.Plan;
			if (plan == null) return Ok(model); //正在訂閱期內或無方案可訂閱

			//查看是否已經有帳單未繳(同方案)
			var bills = await _billsService.FetchByUserAsync(new User { Id = CurrentUserId }, new Plan { Id = plan.Id });
			if (bills.HasItems())
			{
				//帳單有繳的話, 應該在訂閱期內
				//所以應該只會有未繳的
				var unPayedBills = bills.Where(x => !x.Payed).ToList();
				if (unPayedBills.IsNullOrEmpty())
				{
					//沒有未繳帳單,異常
					//例外
					_logger.LogException(new BillPayedButNoCurrentSubscribe(new User { Id = CurrentUserId }, new Plan { Id = plan.Id }));
					return Ok(model);
				}
				else
				{
					//有未繳帳單, 找沒過期的
					var canPayBill = unPayedBills.Where(x => !x.Expired).FirstOrDefault();
					if (canPayBill != null)
					{
						model.Bill = canPayBill.MapViewModel(_mapper);
					}
				}
			}

			//只有進行到這裡,才可建立新訂單
			model.Plan = plan;

			var payways = (await _paysService.FetchPayWaysAsync()).GetOrdered();
			model.PayWays = payways.MapViewModelList(_mapper);

			if (model.Bill == null) model.Bill = new BillViewModel() { PayWayId = payways.FirstOrDefault().Id };

			

			return Ok(model);
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] BillViewModel model)
		{
			//驗證表單
			int planId = model.PlanId;
			var selectedPlan = await _plansService.GetByIdAsync(planId);
			if (selectedPlan == null) throw new EntityNotFoundException(new Plan { Id = planId });

			int payWayId = model.PayWayId;
			var selectedPayWay = _paysService.GetPayWayById(payWayId);
			if (selectedPayWay == null || !selectedPayWay.Active)
			{
				throw new EntityNotFoundException(new PayWay { Id = payWayId });
			}

			//取得訂閱紀錄
			var indexModel = await GetIndexViewAsync();

			var selectedPlanView = selectedPlan.MapViewModel(_mapper);
			//核對方案與金額
			var activePlanView = await CheckForExceptionsAsync(indexModel, selectedPlanView);

			//進行到這裡表示通過驗證. 開始建立Bill
			var bill = new Bill
			{
				UserId = CurrentUserId,
				PlanId = activePlanView.Id,
				Amount = activePlanView.Price,
				HasDiscount = activePlanView.HasDiscount,
				PayWayId = payWayId,
				DeadLine = DateTime.Today.AddDays(_subscribesSettings.BillDaysToExpire).ToEndDate()
			};

			bill = await _billsService.CreateAsync(bill);

			return Ok(bill.MapViewModel(_mapper));
		}

		async Task<SubscribesIndexViewModel> GetIndexViewAsync()
		{
		
			var subscribes = await _subscribesService.FetchByUserAsync(CurrentUserId);

			var model = new SubscribesIndexViewModel();

			if (subscribes.HasItems())
			{
				subscribes = subscribes.GetOrdered();

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



		async Task<PlanViewModel> CheckForExceptionsAsync(SubscribesIndexViewModel indexModel, PlanViewModel selectedPlan)
		{
			if (indexModel.Current != null)
			{
				//目前仍在訂閱期
				throw new CreateWhileCurrentSubscribeExist(CurrentUserId);

			}

			if (indexModel.Plan == null) throw new Exception();
			
			
			var activePlan = indexModel.Plan;

			if (selectedPlan.Id != activePlan.Id)
			{
				//找不到指定的方案
				throw new SelectedPlanDifferentFromActivePlan(selectedPlan.Id, activePlan.Id);
			}

			//查看是否已經有帳單未繳
			var bills = await _billsService.FetchByUserAsync(new User { Id = CurrentUserId }, new Plan { Id = selectedPlan.Id });
			if (bills.HasItems())
			{
				//帳單有繳的話, 應該在訂閱期內
				//所以應該只會有未繳的
				var unPayedBills = bills.Where(x => !x.Payed).ToList();
				if (unPayedBills.IsNullOrEmpty())
				{
					//沒有未繳帳單,異常
					//例外
					_logger.LogException(new BillPayedButNoCurrentSubscribe(new User { Id = CurrentUserId }, new Plan { Id = selectedPlan.Id }));

				}
				//有未繳帳單或帳單異常
				//試圖建立第二張帳單
				throw new TryToCreateSecondValidBill(new User { Id = CurrentUserId }, new Plan { Id = selectedPlan.Id });
				
			}


			if (selectedPlan.Price != activePlan.Price)
			{
				//金額不對
				throw new SelectedPriceDifferentFromActivePlan(selectedPlan.Id, activePlan.Id);
			}

			return activePlan;
		}

	}

	
}
