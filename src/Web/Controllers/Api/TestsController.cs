using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using Newtonsoft.Json;
using System.IO;
using ApplicationCore.Exceptions;
using Microsoft.AspNetCore.SignalR;
using ApplicationCore.Hubs;
using Web.Hubs;
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.Logging;
using Hangfire;

namespace Web.Controllers.Api
{
	public class ATestsController : BaseApiController
	{
		private readonly IWebHostEnvironment _environment;
		private readonly AppSettings _appSettings;
		private readonly IAppLogger _logger;

		private readonly ICloudStorageService _cloudStorageService;
		private readonly AdminSettings _adminSettings;
		private readonly Web.Services.ISubscribesService _subscribesService;

		private readonly IHubContext<NotificationsHub> _notificationHubContext;
		private readonly IHubConnectionManager _userConnectionManager;

		private readonly IMailService _mailService;
		private readonly INoticesService _noticesService;
		private readonly ITestsService _testsService;
		private readonly IDataService _dataService;

		public ATestsController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings, IAppLogger appLogger,
			IHubContext<NotificationsHub> notificationHubContext, IHubConnectionManager userConnectionManager,
			IDataService dataService,
			ICloudStorageService cloudStorageService, IOptions<AdminSettings> adminSettings,
			INoticesService noticesService, IMailService mailService,
			Web.Services.ISubscribesService subscribesService, ITestsService testsService)
		{
			_environment = environment;
			_appSettings = appSettings.Value;
			_logger = appLogger;

			_dataService = dataService;
			_cloudStorageService = cloudStorageService;

			_notificationHubContext = notificationHubContext;
			_userConnectionManager = userConnectionManager;

			_noticesService = noticesService;
			_mailService = mailService;

			_subscribesService = subscribesService;
			_adminSettings = adminSettings.Value;
			_testsService = testsService;


		}

		[HttpGet]
		public ActionResult Index()
		{
			var data = _dataService.FetchYearRecruits();
			var count = data.IsNullOrEmpty() ? 0 : data.Count();
			return Ok(count);
		}

		[HttpPost]
		public async Task<ActionResult> Test([FromBody] AdminRequest model)
		{
			if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
			if (string.IsNullOrEmpty(model.Cmd)) ModelState.AddModelError("cmd", "指令錯誤");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			
			if (model.Cmd.EqualTo("remove-bill"))
			{
				var billView = JsonConvert.DeserializeObject<BillViewModel>(model.Data);
				_testsService.RemoveBill(billView.Id);

				return Ok();
			}
			else if (model.Cmd.EqualTo("remove-subsrcibes"))
			{
				await _testsService.RemoveSubsrcibesFromUserAsync();

				await _testsService.RemoveBillsFromUserAsync();

				return Ok();
			}
			else if (model.Cmd.EqualTo("fake-pay"))
			{
				var tradeResultModel = JsonConvert.DeserializeObject<TradeResultModel>(model.Data);

				var subscribe = await _subscribesService.StorePayAsync(tradeResultModel);
				if (subscribe != null)
				{
					//付款訂閱完成
					BackgroundJob.Enqueue(() => NotifyUserAsync(subscribe.UserId));
				}

				return Ok("1|OK");
			}
			else if (model.Cmd.EqualTo("login"))
			{
				var responseView = await _testsService.LoginAsync(RemoteIpAddress);

				return Ok(responseView);
			}
			else
			{
				ModelState.AddModelError("cmd", "指令錯誤");
				return BadRequest(ModelState);
			}

		}


		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new Exception("Test Exception Throw");
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task NotifyUserAsync(string userId)
		{

			try
			{
				var user = await _subscribesService.FindUserByIdAsync(userId);
				if (user == null) throw new UserNotFoundException(userId);

				//create private notice
				string content = GetMailTemplate(_environment, _appSettings, "subscribe");
				var notice = new Notice
				{
					Title = "您的訂閱會員已經生效",
					Content = content
				};
				await _noticesService.CreateUserNotificationAsync(notice, new List<string> { userId });

				// send email
				string mailSubject = notice.Title;
				string mailTemplate = GetMailTemplate(_environment, _appSettings);
				string mailContent = mailTemplate.Replace(ApplicationCore.Consts.MAILCONTENT, content);

				await _mailService.SendAsync(user.Email, notice.Title, mailContent);


				// send hub notification
				var connections = _userConnectionManager.GetUserConnections(userId);
				if (connections.HasItems())
				{
					foreach (var connectionId in connections)
					{
						await _notificationHubContext.Clients.Client(connectionId).SendAsync(ApplicationCore.Consts.Notifications, userId);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogException(ex);
			}

		}


	}

}