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
using ApplicationCore.Helpers;

namespace Web.Controllers.Api
{
	public class PaysController : BaseApiController
	{
		private readonly IThirdPartyPayService _thirdPartyPayService;
		private readonly IPaysService _paysService;
		private readonly IBillsService _billsService;
		private readonly ISubscribesService _subscribesService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;
		private readonly IAppLogger _logger;

		public PaysController(IThirdPartyPayService thirdPartyPayService, 
			IPaysService paysService, IBillsService billsService, 
			ISubscribesService subscribesService, IUsersService usersService,
			IAppLogger logger, IMapper mapper)
		{
			_thirdPartyPayService = thirdPartyPayService;
			_paysService = paysService;
			_billsService = billsService;
			_subscribesService = subscribesService;
			_usersService = usersService;

			_logger = logger;
			_mapper = mapper;
		}

		[HttpPost("")]
		public async Task<ActionResult> Store()
		{
			try
			{
				var model = _thirdPartyPayService.ResolveTradeResult(Request);

				var pay = _paysService.FindByCode(model.Code);
				if (pay == null) throw new PayNotFound($"code: {model.Code}");

				if (model.Payed) //付款成功的資料
				{
					if (!pay.HasMoney)  //不處理重複發送的資料
					{
						pay.Money = Convert.ToDecimal(model.Amount);
						pay.PayedDate = model.PayedDate.ToDatetimeOrNull();
						pay.TradeNo = model.TradeNo;
						pay.TradeData = model.Data;

						await _paysService.UpdateAsync(pay);

						await OnPayedAsync(pay);
					}

				}
				else
				{
					if (!String.IsNullOrEmpty(model.BankAccount))
					{
						// 獲取ATM虛擬帳號
						pay.BankCode = model.BankCode;
						pay.BankAccount = model.BankAccount;

						if (!String.IsNullOrEmpty(model.PayWay)) pay.PayWay = model.PayWay;

						await _paysService.UpdateAsync(pay);

					}

				}

				_logger.LogInfo("ResolveTradeResult: 1|OK");
				return Ok("1|OK");

			}
			catch (Exception ex)
			{
				_logger.LogException(ex);

				if (ex is EcPayTradeFeedBackFailed)
				{
					// rtnCode 不是 1 也不是 2
					_logger.LogInfo("ResolveTradeResult: 1|OK");
					return Ok("1|OK");
				}
				else if (ex is EcPayTradeFeedBackError)
				{
					_logger.LogInfo($"ResolveTradeResult: 0|{ex.Message}");
					return Ok($"0|{ex.Message}");
				}
				else
				{
					throw ex;
				}

			}
			
		}



		[HttpPost("xxstore")]
        public async Task<ActionResult> xxStore([FromBody] TradeResultModel model)
        {
			var pay = _paysService.FindByCode(model.Code);
			if (pay == null) throw new PayNotFound($"code: {model.Code}");

			if (model.Payed) //付款成功的資料
			{
				if (!pay.HasMoney)  //不處理重複發送的資料
				{
					pay.Money = Convert.ToDecimal(model.Amount);
					pay.PayedDate = model.PayedDate.ToDatetimeOrNull();
					pay.TradeNo = model.TradeNo;
					pay.TradeData = model.Data;

					await _paysService.UpdateAsync(pay);

					await OnPayedAsync(pay);
				}
				
			}
			else
			{
				if (!String.IsNullOrEmpty(model.BankAccount))
				{
					// 獲取ATM虛擬帳號
					pay.BankCode = model.BankCode;
					pay.BankAccount = model.BankAccount;

					if (!String.IsNullOrEmpty(model.PayWay)) pay.PayWay = model.PayWay;

					await _paysService.UpdateAsync(pay);

				}
				
			}


			return Ok();
        }

		async Task OnPayedAsync(Pay pay)  //當付款成功紀錄後執行
		{
			var bill = _billsService.GetById(pay.BillId);
			if (bill == null) throw new BillNotFoundWhilePay($"bill id: {pay.BillId}");

			if (!bill.Payed) throw new NotPayedAfterPay(bill, pay);


			//建立 Subscribe
			var subscribe = _subscribesService.Find(bill);
			if (subscribe == null) subscribe = await _subscribesService.CreateAsync(Subscribe.Create(bill));


			//加入角色
			if (subscribe.Active) await _usersService.AddSubscriberRoleAsync(subscribe.UserId);
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
