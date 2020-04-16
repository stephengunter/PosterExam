using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Views;
using ApplicationCore.Settings;

namespace Web.Controllers
{
	[Route("[controller]")]
	public abstract class BaseController : ControllerBase
	{
		protected string RemoteIpAddress => Request.HttpContext.Connection.RemoteIpAddress?.ToString();

		protected string CurrentUserName => User.Claims.IsNullOrEmpty() ? "" : User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;

		protected string CurrentUserId => User.Claims.IsNullOrEmpty() ? "" : User.Claims.Where(c => c.Type == "id").FirstOrDefault().Value;

		protected IActionResult RequestError(string key, string msg)
		{
			ModelState.AddModelError(key, msg);
			return BadRequest(ModelState);
		}

	}

	
	[Route("api/[controller]")]
	public abstract class BaseApiController : BaseController
	{
		
	}

	
	[Route("admin/[controller]")]
	[Authorize(Policy = "Admin")]
	[Authorize]
	public class BaseAdminController : BaseController
	{

		protected void ValidateRequest(AdminRequest model, AdminSettings adminSettings)
		{
			if (model.Key != adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");

		}
	}
}
