using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public abstract class BaseController : ControllerBase
	{
		protected string RemoteIpAddress => Request.HttpContext.Connection.RemoteIpAddress?.ToString();

		protected string CurrentUserName => User.Claims.Where(c => c.Type == "sub").FirstOrDefault().Value;

		protected string CurrentUserId => User.Claims.Where(c => c.Type == "id").FirstOrDefault().Value;

		protected IActionResult RequestError(string key, string msg)
		{
			ModelState.AddModelError(key, msg);
			return BadRequest(ModelState);
		}
		
	}
}
