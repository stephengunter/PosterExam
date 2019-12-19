using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models;
using ApplicationCore.Auth;
using Microsoft.Extensions.Options;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Newtonsoft.Json;
using Google.Apis.Auth;
using ApplicationCore.Settings;
using ApplicationCore.Exceptions;

namespace Web.Controllers
{
	public class AuthController : BaseController
	{

		private readonly UserManager<User> _userManager;
		private readonly IAuthService _authService;

		//private static readonly HttpClient Client = new HttpClient();

		public AuthController(UserManager<User> userManager, IAuthService authService)
		{
			_userManager = userManager;
			_authService = authService;
		}

		//POST api/auth/refreshtoken
		[HttpPost("refreshtoken")]
		public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
		{
			string userId = _authService.GetUserIdFromToken(model.accessToken);			

			ValidateRequest(model, userId);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var user = await _userManager.FindByIdAsync(userId);
			var roles = await _userManager.GetRolesAsync(user);

			var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, roles);

			return Ok(responseView);

		}

		void ValidateRequest(RefreshTokenRequest model, string userId)
		{
			bool isValid = _authService.IsValidRefreshToken(model.refreshToken, userId);
			if(!isValid) ModelState.AddModelError("token", "身分驗證失敗. 請重新登入");

		}



	}

}