using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Views;
using ApplicationCore.Settings;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Cors;
using System.Data.SqlClient;

namespace Web.Controllers
{
	[EnableCors("Global")]
	[Route("[controller]")]
	public abstract class BaseController : ControllerBase
	{
		protected string RemoteIpAddress => Request.HttpContext.Connection.RemoteIpAddress?.ToString();

		protected string CurrentUserName => User.Claims.IsNullOrEmpty() ? "" : User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;

		protected string CurrentUserId => User.Claims.IsNullOrEmpty() ? "" : User.Claims.Where(c => c.Type == "id").FirstOrDefault().Value;

		protected IEnumerable<string> CurrentUseRoles
		{
			get
			{
				var entity = User.Claims.Where(c => c.Type == "roles").FirstOrDefault();
				if (entity == null) return null;
				return entity.Value.Split(',');
			}
			
		}

		protected bool CurrentUserIsSubscriber
		{
			get
			{
				var roles = CurrentUseRoles;
				if (roles.IsNullOrEmpty()) return false;
				var match = roles.Where(r => r.ToUpper() == ApplicationCore.Consts.SubscriberRoleName.ToUpper()).FirstOrDefault();

				return match != null;
			}
		}

		protected IActionResult RequestError(string key, string msg)
		{
			ModelState.AddModelError(key, msg);
			return BadRequest(ModelState);
		}

		protected string MailTemplatePath(IWebHostEnvironment environment, AppSettings appSettings)
			=> Path.Combine(environment.WebRootPath, appSettings.TemplatePath.HasValue() ? appSettings.TemplatePath : "templates");


		protected string GetMailTemplate(IWebHostEnvironment environment, AppSettings appSettings, string name = "default")
		{
			var pathToFile = Path.Combine(MailTemplatePath(environment, appSettings), $"{name}.html");
			if (!System.IO.File.Exists(pathToFile)) throw new Exception("email template file not found: " + pathToFile);

			string body = "";
			using (StreamReader reader = System.IO.File.OpenText(pathToFile))
			{
				body = reader.ReadToEnd();
			}

			return body.Replace("APPNAME", appSettings.Title).Replace("APPURL", appSettings.ClientUrl);

		}
	}

	[EnableCors("Api")]
	[Route("api/[controller]")]
	public abstract class BaseApiController : BaseController
	{
		
	}

	[EnableCors("Admin")]
	[Route("admin/[controller]")]
	[Authorize(Policy = "Admin")]
	public class BaseAdminController : BaseController
	{
		protected string BackupFolder(AdminSettings adminSettings)
		{
			var path = Path.Combine(adminSettings.BackupPath, DateTime.Today.ToDateNumber().ToString());
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);

			return path;
		}

		protected string GetDbName(string connectionString) => new SqlConnectionStringBuilder(connectionString).InitialCatalog;

		protected void ValidateRequest(AdminRequest model, AdminSettings adminSettings)
		{
			if (model.Key != adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");

		}
	}
}
