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

		private readonly Web.Services.ISubscribesService _subscribesService;

		public ATestsController(IOptions<EcPaySettings> ecPaySettings, IPaysService paysService, IWebHostEnvironment environment, IOptions<AppSettings> appSettings, 
			IOptions<AdminSettings> adminSettings, ITestsService testsService, IMailService mailService,
			Web.Services.ISubscribesService subscribesService)
		{
			_ecpaySettings = ecPaySettings.Value;
			_paysService = paysService;
			_environment = environment;
			_appSettings = appSettings.Value;

			_adminSettings = adminSettings.Value;
			_testsService = testsService;
			_mailService = mailService;

			_subscribesService = subscribesService;
		}
		[HttpGet]
		public ActionResult Index(string val)
		{
			return Ok(_ecpaySettings.MerchantID);
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