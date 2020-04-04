using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	[Route("api/admin/[controller]")]
	[Authorize(Policy = "Admin")]
	[Authorize]
	[ApiController]
	public class BaseAdminController : ControllerBase
	{
		protected string RemoteIpAddress => Request.HttpContext.Connection.RemoteIpAddress?.ToString();

		protected string CurrentUserName => User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;

		protected string CurrentUserId => User.Claims.Where(c => c.Type == "id").FirstOrDefault().Value;

		protected IActionResult RequestError(string key, string msg)
		{
			ModelState.AddModelError(key, msg);
			return BadRequest(ModelState);
		}

		protected void ValidateRequest(AdminRequest model, AdminSettings adminSettings)
		{
			if (model.Key != adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");

		}
	}
}
