using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.Paging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ApplicationCore.Settings;

namespace Web.Controllers.Api
{
	public class PhotoController : BaseController
	{
		private readonly IHostingEnvironment _environment;
		private readonly AppSettings _appSettings;

		public PhotoController(IHostingEnvironment environment, IOptions<AppSettings> appSettings)
		{
			_environment = environment;
			_appSettings = appSettings.Value;
		}

		string UploadFilesPath => Path.Combine(_environment.WebRootPath, _appSettings.UploadPath);

		public IActionResult Index(int width, int height, string type, string path)
		{
			//檢查檔案路徑
			string imgSourcePath = Path.Combine(UploadFilesPath, path);
			if (!System.IO.File.Exists(imgSourcePath)) throw new Exception(String.Format("圖片路徑無效:{0}", imgSourcePath));


			string extension = (Path.HasExtension(imgSourcePath)) ?
											  System.IO.Path.GetExtension(imgSourcePath).Substring(1).ToLower() :
											  string.Empty;
			if (!("jpg".Equals(extension) || "gif".Equals(extension) || "png".Equals(extension)))
			{
				throw new Exception(String.Format("圖片格式錯誤:{0}", imgSourcePath));
			}

			// 長寬數值不正確時, 回傳原圖
			if (width <= 0 || height <= 0)
			{
				return SendOriginalImage(imgSourcePath);

			}

			Image imgSource = Image.FromStream(new MemoryStream(System.IO.File.ReadAllBytes(imgSourcePath)));
			Image imgResized = null;
			if (type == "crop") imgResized = ImageResizer.GetCropedImage(imgSource, width, height);
			else imgResized = ImageResizer.GetResizedImage(imgSource, width, height);

			if (imgResized == null)
			{
				return SendOriginalImage(imgSourcePath);
			}
			else
			{
				Stream outputStream = new MemoryStream();

				imgResized.Save(outputStream, ImageFormat.Jpeg);
				outputStream.Seek(0, SeekOrigin.Begin);

				return this.File(outputStream, "image/jpeg");
			}
		}


		// 傳回原始圖片
		private IActionResult SendOriginalImage(string imgSourcePath)
		{
			string type = "image/jpeg";

			string ext = Path.GetExtension(imgSourcePath).ToLower();
			if (ext == "png") type = "image/png";
			else if (ext == "gif") type = "image/gif";

			var image = System.IO.File.OpenRead(imgSourcePath);
			return File(image, type);

		}


		
	}
}