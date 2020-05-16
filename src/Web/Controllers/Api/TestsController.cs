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
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.ViewServices;
using AutoMapper;

namespace Web.Controllers.Api
{
	public class ATestsController : BaseApiController
	{
		private readonly EcPaySettings _ecpaySettings;
		private readonly IPaysService _paysService;
		private readonly IWebHostEnvironment _environment;
		private readonly AppSettings _appSettings;
		private readonly AdminSettings _adminSettings;
		private readonly ITestsService _testsService;
		private readonly IMailService _mailService;

		private readonly INoticesService _noticesService;

		private readonly IMapper _mapper;

		private readonly Web.Services.ISubscribesService _subscribesService;

		public ATestsController(IOptions<EcPaySettings> ecPaySettings, IPaysService paysService, IWebHostEnvironment environment, IOptions<AppSettings> appSettings, 
			IOptions<AdminSettings> adminSettings, ITestsService testsService, IMailService mailService,
			Web.Services.ISubscribesService subscribesService, INoticesService noticesService, IMapper mapper)
		{
			_ecpaySettings = ecPaySettings.Value;
			_paysService = paysService;
			_environment = environment;
			_appSettings = appSettings.Value;

			_adminSettings = adminSettings.Value;
			_testsService = testsService;
			_mailService = mailService;

			_subscribesService = subscribesService;

			_noticesService = noticesService;
			_mapper = mapper;
		}
		[HttpGet]
		public async Task<ActionResult> Index()
		{
			var receivers = await _noticesService.FetchUserNotificationsAsync(_adminSettings.Id);

			return Ok(receivers.MapViewModelList(_mapper));
			//return Ok(_ecpaySettings.MerchantID);
		}

		async Task CreateFakeUserNoticesAsync()
		{
			var notices = new List<Notice>
			{
				new Notice { Title = "娃哈哈回应开奶茶店称由授权合作伙伴运营", Content = "娃哈哈回应开奶茶店 称由授权合作伙伴运营" },
				new Notice { Title = "平安健康涉嫌侵犯商业秘密后续：雪扬公司称已立案", Content = "平安健康涉嫌侵犯商业秘密后续：雪扬公司称已立案" },
				new Notice { Title = "新冠疫情暴露世界领导力危机：当心跌入金德尔伯格陷阱", Content = "新冠疫情暴露世界领导力危机：当心跌入金德尔伯格陷阱" },
			};

			var userIds = new List<string> { _adminSettings.Id };

			foreach (var item in notices)
			{
				await _noticesService.CreateUserNotificationAsync(item, userIds);
			}
		}


		[HttpPost]
		public async Task<ActionResult> Test([FromBody] AdminRequest model)
		{
			if (model.Key != _adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");
			if (string.IsNullOrEmpty(model.Cmd)) ModelState.AddModelError("cmd", "指令錯誤");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			if (model.Cmd.EqualTo("SendEmail"))
			{
				string mailTo = _adminSettings.Email;
				var template = GetMailTemplate(_environment, _appSettings);
				string subject = "測試email主旨";
				string text = "測試email內容";
				var content = template.Replace("MAILCONTENT", text);

				await _mailService.SendAsync(mailTo, subject, content, text);
				
			}
			else if (model.Cmd.EqualTo("remove-bill"))
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
			else if (model.Cmd.EqualTo("login"))
			{
				var responseView = await _testsService.LoginAsync(RemoteIpAddress);

				return Ok(responseView);
			}
			else if (model.Cmd.EqualTo("fake-pay"))
			{
				var tradeResultModel = JsonConvert.DeserializeObject<TradeResultModel>(model.Data);

				await _subscribesService.StorePayAsync(tradeResultModel);

				return Ok("1|OK");
			}
			else if (model.Cmd.EqualTo("fake-notices"))
			{
				var models = JsonConvert.DeserializeObject<List<NoticeViewModel>>(model.Data);

				
				foreach (var item in models)
				{
					var entity = item.MapEntity(_mapper, _adminSettings.Id);
					entity.CreatedAt = item.CreatedAt;
					await _noticesService.CreateAsync(entity);
				}

				return Ok(models);
			}
			else
			{
				ModelState.AddModelError("cmd", "指令錯誤");
				return BadRequest(ModelState);
			}

			

			return Ok();

		}

		


		[HttpGet("ex")]
		public async Task<ActionResult> Ex()
		{
			throw new Exception("Test Exception Throw");
		}


	}

}