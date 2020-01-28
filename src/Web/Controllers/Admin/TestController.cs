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
using AutoMapper;
using ApplicationCore.ViewServices;
using ApplicationCore.Logging;
using Web.Helpers.ViewServices;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{
		DefaultContext _context;
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;

		public ATestController(DefaultContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
		}
	

		[HttpGet("")]
		public async Task<ActionResult> Index(int id)
		{
			var users = _userManager.Users;
			var user = users.FirstOrDefault();

			var userRoles = _context.UserRoles.Where(x => x.UserId == user.Id);

			return Ok(userRoles);
		}


	}
}
