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
	public class OAuthController : BaseController
	{
		private readonly UserManager<User> _userManager;
		private readonly IAuthService _authService;

		private static readonly HttpClient Client = new HttpClient();

		public OAuthController(UserManager<User> userManager, IAuthService authService)
		{
			_userManager = userManager;
			_authService = authService;
		}
		

		//POST api/oauth/google
		[HttpPost("google")]
		public async Task<ActionResult> Google([FromBody] OAuthLoginRequest model)
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(model.token, new GoogleJsonWebSignature.ValidationSettings());

			var user = await _userManager.FindByEmailAsync(payload.Email);

			if (user == null) user = await CreateUserAsync(payload.Email, payload.Name);

			var oAuth = new OAuth
			{
				OAuthId = payload.Subject,
				Provider = OAuthProvider.Google,
				UserId = user.Id,
				PictureUrl = payload.Picture
			};

			await _authService.CreateUpdateUserOAuthAsync(user.Id, oAuth);

			var roles = await _userManager.GetRolesAsync(user);



			var responseView = await _authService.CreateTokenAsync(RemoteIpAddress, user, oAuth, roles);

			return Ok(responseView);
		}





		async Task<User> CreateUserAsync(string email, string name)
		{
			var result = await _userManager.CreateAsync(new User
			{
				Email = email,
				UserName = email,
				Name = name,
				EmailConfirmed = true
			});

			if (!result.Succeeded) OnCreateUserError(result);

			return await _userManager.FindByEmailAsync(email);
		}

		void OnCreateUserError(IdentityResult result)
		{
			var error = result.Errors.FirstOrDefault();
			throw new CreateUserException($"{error.Code} : {error.Description}");
		}


	}

}