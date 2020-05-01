using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using AutoMapper;
using Web.Models;
using ApplicationCore.Logging;
using ApplicationCore.Exceptions;
using System.Collections;

namespace Web.Controllers.Api
{
	public class PaysController : BaseApiController
	{
		private readonly IPaysService _paysService;
		private readonly IBillsService _billsService;
		private readonly ISubscribesService _subscribesService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;


		public PaysController(IPaysService paysService, IBillsService billsService,
			ISubscribesService subscribesService, IUsersService usersService,
			IAppLogger logger, IMapper mapper)
		{

			_paysService = paysService;
			_billsService = billsService;
			_subscribesService = subscribesService;
			_usersService = usersService;

			_logger = logger;
			_mapper = mapper;
		}


        [HttpPost("")]
        public ActionResult Store([FromBody] EcPayTradeResultModel model)
        {

			


			return Ok("1|OK");
        }

        

		[HttpPost("__store")]
		public async Task<ActionResult> __Store([FromBody] PayForm form)
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
				PayWay = payway.Code
			});

			var subscribe = await CreateSubscribeAsync(pay);
			if (subscribe != null)
			{
				if (subscribe.Active)
				{
					// add user to role
					try
					{
						await _usersService.AddSubscriberRoleAsync(subscribe.UserId);
					}
					catch (Exception ex)
					{

						_logger.LogException(ex);
					}
				}
			}

			return Ok();
		}

		async Task<Subscribe> CreateSubscribeAsync(Pay pay)
		{
			Bill bill = null;
			try
			{
				bill = _billsService.GetById(pay.BillId);

				if (bill == null)
				{
					_logger.LogException(new EntityNotFoundException(new Bill { Id = pay.BillId }));
					return null;
				}


				if (bill.Payed) return await _subscribesService.CreateAsync(Subscribe.Create(bill));
				else
				{
					//金額不對, 異常, 帳單應該是已支付
					_logger.LogException(new NotPayedAfterPay(bill, pay));
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(new CreateSubscribeFailed(bill));
				_logger.LogException(ex);

				return null;
			}
		}

	}

	
}
