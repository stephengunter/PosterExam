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
using ApplicationCore;

namespace Web.Controllers.Api
{
	[Authorize]
	public class BillsController : BaseApiController
	{
		private readonly IAppLogger _logger;

		private readonly IBillsService _billsService;
		private readonly IPaysService _paysService;
		private readonly IThirdPartyPayService _thirdPartyPayService;
		private readonly IMapper _mapper;

		
		public BillsController(IAppLogger logger, IBillsService billsService, IPaysService paysService, 
			IThirdPartyPayService thirdPartyPayService, IMapper mapper)
		{
			_logger = logger;

			_billsService = billsService;
			_paysService = paysService;
			_thirdPartyPayService = thirdPartyPayService;
			_mapper = mapper;
		}

		[HttpGet("edit/{id}")]
		public async Task<ActionResult> Edit(int id)
		{
			var bill = _billsService.GetById(id);
			if (bill == null) return NotFound();
			if (bill.Payed) return NotFound(); //已經支付, 應該去Details

			var form = new BillEditForm()
			{
				Bill = bill.MapViewModel(_mapper)
			};

			bool canPay = !bill.Expired;
			if (canPay)
			{
				var bills = await _billsService.FetchByUserAsync(new User { Id = CurrentUserId }, new Plan { Id = bill.PlanId });
				if (bills.HasItems())
				{
					//查看是否已經有同方案, 已支付的帳單
					var payedBill = bills.Where(x => x.Payed).FirstOrDefault();
					if (payedBill != null) canPay = false;
				}
			}

			if (canPay)
			{
				var payways = (await _paysService.FetchPayWaysAsync()).GetOrdered();
				form.PayWays = payways.MapViewModelList(_mapper);
			}

			return Ok(form);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult> Details(int id)
		{
			var bill = _billsService.GetById(id);
			if (bill == null) return NotFound();

			var payways = await _paysService.FetchPayWaysAsync();

			var model = bill.MapViewModel(_mapper, payways);

			return Ok(model);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> Update(int id, [FromBody] BillViewModel model)
		{
			// BeginPay 支付帳單
			var existingBill = _billsService.GetById(id);
			if (existingBill == null) return NotFound();

			int paywayId = model.PayWayId;
			var payway = _paysService.GetPayWayById(paywayId);
			if (payway == null) throw new EntityNotFoundException(new PayWay { Id = paywayId });


			if (existingBill.Payed)
			{
				ModelState.AddModelError("payed", "此訂單已經支付過了");
				return BadRequest(ModelState);
			}

			if (existingBill.Expired)
			{
				ModelState.AddModelError("expired", "訂單已過繳款期限");
				return BadRequest(ModelState);
			}

			var pay = Pay.Create(existingBill, payway, ThirdPartyPayment.EcPay);

			//await _paysService.CreateAsync(pay);

			//existingBill.Code = TickId.Create();
			//existingBill.PayWayId = payway.Id;
			//await _billsService.UpdateAsync(existingBill);

			try
			{
				var amount = Convert.ToInt32(existingBill.NeedPayMoney);
				var ecPayTradeModel = await _thirdPartyPayService.CreateEcPayTradeAsync(pay, amount);

				await _paysService.CreateAsync(pay);

				if (existingBill.PayWayId != paywayId)
				{
					existingBill.PayWayId = paywayId;
					await _billsService.UpdateAsync(existingBill);
				}

				return Ok(ecPayTradeModel);
			}
			catch (Exception ex)
			{
				// Create ThirdParty Trade Failed
				_logger.LogException(ex);
				throw ex;
			}
		}

	}

	
}
