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
		private readonly IPaysService _paysService;
		private readonly IMapper _mapper;

		
		public BillsController(IBillsService billsService, IPaysService paysService,
			IMapper mapper)
		{
			
			_billsService = billsService;
			_paysService = paysService;
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

	}

	
}
