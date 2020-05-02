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
		private readonly IHttpClientFactory _clientFactory;

		private readonly IAppLogger _logger;
		private readonly AppSettings _appSettings;
		private readonly IThirdPartyPayService _thirdPartyPayService;

		private readonly IUsersService _usersService;
		public ATestController(IOptions<AppSettings> appSettings, IThirdPartyPayService thirdPartyPayService,
			IUsersService usersService, IAppLogger logger, IHttpClientFactory clientFactory)
		{
			_appSettings = appSettings.Value;
			_thirdPartyPayService = thirdPartyPayService;
			_usersService = usersService;
			_logger = logger;
			_clientFactory = clientFactory;
		}

		string PayStoreUrl => $"{_appSettings.BackendUrl}/api/pay";

		

		[HttpGet]
		public async Task<ActionResult> Index(string code)
		{

			return Ok();

		}


		[HttpGet("user")]
		public async Task<ActionResult> UserRole()
		{
			var user = await _usersService.FindSubscriberAsync(CurrentUserId);
			if (user == null)
			{
				await _usersService.AddSubscriberRoleAsync(CurrentUserId);
			}
			else
			{
				await _usersService.RemoveSubscriberRoleAsync(CurrentUserId);
			}

			return Ok();

		}

		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new ExamNotRecruitQuestionSelected();
		}


	}

}