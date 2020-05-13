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
using Microsoft.AspNetCore.Cors;

namespace Web.Controllers.Api
{
	[EnableCors("EcPay")]
	public class PaysController : BaseApiController
	{
		private readonly Web.Services.ISubscribesService _subscribesService;
		private readonly IAppLogger _logger;

		public PaysController(Web.Services.ISubscribesService subscribesService, IAppLogger appLogger)
		{
			_subscribesService = subscribesService;
			_logger = appLogger;
		}

		[HttpPost("")]
		public async Task<ActionResult> Store()
		{
			TradeResultModel tradeResultModel = null;
			try
			{
				tradeResultModel = _subscribesService.ResolveTradeResult(Request);
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

			await _subscribesService.StorePayAsync(tradeResultModel);

			_logger.LogInfo("ResolveTradeResult: 1|OK");

			return Ok("1|OK");
			

		}


	}

	
}
