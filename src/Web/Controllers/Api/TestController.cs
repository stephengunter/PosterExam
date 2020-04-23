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
	public class ATestController : BaseApiController
	{
		private readonly SubscribesSettings _subscribesSettings;

		private readonly ISubscribesService _subscribesService;
		private readonly IPlansService _plansService;
		private readonly IPaysService _paysService;
		private readonly IBillsService _billsService;
		private readonly IMapper _mapper;


		public ATestController(IOptions<SubscribesSettings> subscribesSettings, ISubscribesService subscribesService, IPlansService plansService,
			IPaysService paysService, IBillsService billsService, IMapper mapper)
		{
			_subscribesSettings = subscribesSettings.Value;

			_subscribesService = subscribesService;
			_plansService = plansService;
			_paysService = paysService;
			_billsService = billsService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var pay = new Pay
			{
				BillId = 2,
				Code = Guid.NewGuid().ToString("N"),
				Money = 190,
				PayWayId = 1
			};

			pay = await _paysService.CreateAsync(pay);

			return Ok(pay.MapViewModel(_mapper));


			//string userId = "695f31b3-74d7-4fa2-b877-898c31997b00";

			//var activePlan = await FindPlanAsync();

			////開始建立Bill
			//var bill = new Bill
			//{
			//	UserId = userId,
			//	PlanId = activePlan.Id,
			//	Amount = activePlan.Money,
			//	HasDiscount = false,
			//	PayWayId = 1,
			//	DeadLine = DateTime.Today.AddDays(10).ToEndDate()
			//};

			//bill = await _billsService.CreateAsync(bill);

			//return Ok(bill.MapViewModel(_mapper));
		}

		async Task<Pay> CreatePayAsync()
		{
			var pay = new Pay
			{
				BillId = 2,
				Code = Guid.NewGuid().ToString("N"),
				Money = 190,
				PayWayId = 1
			};

			return await _paysService.CreateAsync(pay);


		}

		async Task<Plan> FindPlanAsync()
		{
			bool active = true;
			var activePlans = await _plansService.FetchAsync(active);
			
			return activePlans.FirstOrDefault();
		}

		void CreateBill()
		{ 
			
		}

		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new ExamNotRecruitQuestionSelected();
		}


	}

}