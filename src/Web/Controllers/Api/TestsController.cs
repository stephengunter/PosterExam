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

namespace Web.Controllers.Api
{
	public class ATestsController : BaseApiController
	{
		private readonly ICloudStorageService _cloudStorageService;
		private readonly AdminSettings _adminSettings;
		private readonly Web.Services.ISubscribesService _subscribesService;
		private readonly ITestsService _testsService;

		public ATestsController(
			ICloudStorageService cloudStorageService, IOptions<AdminSettings> adminSettings,
			Web.Services.ISubscribesService subscribesService, ITestsService testsService)
		{

			_cloudStorageService = cloudStorageService;
			_subscribesService = subscribesService;
			_adminSettings = adminSettings.Value;
			_testsService = testsService;
		}

		[HttpGet]
		public async Task<ActionResult> Index()
		{
			string result =  await _cloudStorageService.UploadFileAsync(@"C:\testApp\Gunter_in_Sign.png", @"folder1/Gunter_in_Sign.png");
			return Ok(result);
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


	}

}