using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using ApplicationCore.Settings;

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{

		private readonly IAttachmentsService _attachmentsService;
		private readonly IHostingEnvironment _environment;
		private readonly AppSettings _appSettings;

		public ATestController(IHostingEnvironment environment, IOptions<AppSettings> appSettings, IAttachmentsService attachmentsService)
		{
			_environment = environment;
			_appSettings = appSettings.Value;
			_attachmentsService = attachmentsService;
		}

		string UploadFilesPath => Path.Combine(_environment.WebRootPath, _appSettings.UploadPath);



		[HttpPost("")]
		public async Task<IActionResult> Store([FromForm] UploadForm form)
		{
			PostType postType = form.GetPostType();
			if (postType == PostType.Unknown)
			{
				ModelState.AddModelError("PostType", "錯誤的PostType");
				return BadRequest(ModelState);
			}

			var attachments = new List<UploadFile>();

			foreach (var file in form.Files)
			{
				if (file.Length > 0)
				{
					//var attachment = await _attachmentsService.FindByNameAsync(file.FileName, postType, form.PostId);
					//if (attachment == null) throw new Exception(String.Format("attachmentService.FindByName({0},{1})", file.FileName, form.PostId));

					var attachment = new UploadFile();

					var upload = await SaveFile(file);
					attachment.PostType = postType;
					attachment.Type = upload.Type;
					attachment.Path = upload.Path;

					switch (upload.Type)
					{
						case ".jpg":
						case ".jpeg":
						case ".png":
						case ".gif":
							var image = Image.Load(file.OpenReadStream());
							attachment.Width = image.Width;
							attachment.Height = image.Height;
							attachment.PreviewPath = upload.Path;
							break;
					}

					attachments.Add(attachment);
				}
			}


			_attachmentsService.UpdateRange(attachments);
			return Ok();

		}


		async Task<UploadFile> SaveFile(IFormFile file)
		{
			//檢查檔案路徑
			string folderName = DateTime.Now.ToString("yyyyMMdd");
			string folderPath = Path.Combine(this.UploadFilesPath, folderName);
			if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

			string extension = Path.GetExtension(file.FileName).ToLower();

			string fileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
			string filePath = Path.Combine(folderPath, fileName);
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
			}


			var entity = new UploadFile()
			{
				Type = extension,
				Path = folderName + "/" + fileName
			};

			return entity;
		}




	}
}
