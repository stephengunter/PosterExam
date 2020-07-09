using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Data.SqlClient;

namespace Web.Controllers.Admin
{
	public class CloudsController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly ICloudStorageService _cloudStorageService;

		public CloudsController(IOptions<AdminSettings> adminSettings, ICloudStorageService cloudStorageService)
		{
			_adminSettings = adminSettings.Value;
			_cloudStorageService = cloudStorageService;
		}

		
		[HttpPost("")]
		public async Task<ActionResult> DbBackups([FromBody] AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var folderPath = BackupFolder(_adminSettings);
			string storageFolder = DateTime.Today.ToDateNumber().ToString();

			foreach (var filePath in Directory.GetFiles(folderPath))
			{
				var fileInfo = new FileInfo(filePath);
				await _cloudStorageService.UploadFileAsync(filePath, $"{storageFolder}/{fileInfo.Name}");
			}
			

			return Ok();
		}


	}
}
