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
	public class PaysController : BaseApiController
	{
		private readonly IPaysService _paysService;
		private readonly IBillsService _billsService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;

		
		public PaysController(IPaysService paysService, IBillsService billsService,
			IAppLogger logger, IMapper mapper)
		{
			
			_paysService = paysService;
			_billsService = billsService;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpPost("")]
		public async Task<ActionResult> Store([FromBody] PayForm form)
		{
			string code = form.Id;
			var exist = _paysService.FindByCode(code);
			if (exist != null)
			{
				_logger.LogException(new PayRecordAlreadyExistWhilePay($"code: {code}"));
				return Ok();
			}

			int billId = form.BillId;
			var bill = _billsService.GetById(billId);
			if (bill == null)
			{
				_logger.LogException(new BillNotFoundWhilePay($"bill id: {billId}"));

				ModelState.AddModelError("billId", "帳單不存在");
				return BadRequest(ModelState);
			}

			if (bill.Payed)
			{
				_logger.LogException(new BillPayedAlreadyWhilePay($"bill id: {billId} , code: {code}"));

				ModelState.AddModelError("billId", "帳單已經支付");
				return BadRequest(ModelState);
			}

			var payway = _paysService.FindPayWayByCode(form.PayWay);
			if (payway == null)
			{
				_logger.LogException(new PayWayNotFound(form.PayWay));

				ModelState.AddModelError("payWay", "PayWayNotFound");
				return BadRequest(ModelState);
			}


			var money = Convert.ToDecimal(form.Money);
			if (money != bill.NeedPayMoney)
			{
				_logger.LogException(new PayMoneyNotEqualBillNeedPay($"bill id: {billId} , code: {code}"));
				//金額不對, 但繼續執行將支付建檔
			}

			var pay = await _paysService.CreateAsync(new Pay
			{
				Code = code,
				BillId = billId,
				Money = money,
				PayWayId = payway.Id
			});

			return Ok();
		}

	}

	
}
