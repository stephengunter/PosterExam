using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Views;
using ApplicationCore.Settings;
using System.Collections.Generic;

namespace Web.Controllers
{
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
