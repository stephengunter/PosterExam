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
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;
using System.Text;
using ApplicationCore;

namespace Web.Controllers.Api
{
	//[Authorize]
	public class ATestController : BaseApiController
	{
		

		private readonly IAppLogger _logger;
		private readonly AppSettings _appSettings;
		private readonly IThirdPartyPayService _thirdPartyPayService;
		private readonly IPaysService _paysService;
		private readonly ISubscribesService _subscribesService;
		private readonly IMailService _mailService;

		private readonly IUsersService _usersService;
		public ATestController(IOptions<AppSettings> appSettings, IThirdPartyPayService thirdPartyPayService,
			IUsersService usersService, IAppLogger logger, IPaysService paysService,
			ISubscribesService subscribesService, IMailService mailService)
		{
			_appSettings = appSettings.Value;
			_thirdPartyPayService = thirdPartyPayService;
			_usersService = usersService;
			_logger = logger;
			
			_subscribesService = subscribesService;
			_paysService = paysService;
			_mailService = mailService;
		}

		string PayStoreUrl => $"{_appSettings.BackendUrl}/api/pay";

		

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			

			return Ok(_appSettings.Title);

		}

		async Task TestMail()
		{
			var mails = new List<string>
			{
				"traders.com.tw@gmail.com", "poster.examlearner@gmail.com"
			};

			try
			{
				foreach (var item in mails)
				{
					await _mailService.SendAsync(item, "test subject", "<h1>Test HTML </h1>", "test plain text");
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);

			}

		}

		async Task RemoveSubsrcibe(int id)
		{
			var subscribe = _subscribesService.GetById(id);

			string userId = subscribe.UserId;
			await _subscribesService.RemoveAsync(subscribe);


			await _usersService.RemoveSubscriberRoleAsync(userId);

		}


		//[HttpGet("user")]
		//public async Task<ActionResult> UserRole()
		//{
		//	var user = await _usersService.FindSubscriberAsync(CurrentUserId);
		//	if (user == null)
		//	{
		//		await _usersService.AddSubscriberRoleAsync(CurrentUserId);
		//	}
		//	else
		//	{
		//		await _usersService.RemoveSubscriberRoleAsync(CurrentUserId);
		//	}

		//	return Ok();

		//}

		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new ExamNotRecruitQuestionSelected();
		}


	}

}