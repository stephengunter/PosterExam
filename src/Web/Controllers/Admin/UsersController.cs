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
using Web.Models;
using Web.Helpers;
using Web.Controllers;

namespace Web.Controllers.Admin
{
	public class UsersController : BaseAdminController
	{
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;

		public UsersController(IUsersService usersService, IMapper mapper)
		{
			_usersService = usersService;
			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(string role = "", string keyword = "", int page = 1, int pageSize = 10)
		{
			var model = new UsersAdminModel();

			if (page < 1) //初次載入頁面
			{
				var roles = _usersService.FetchRoles();
				model.LoadRolesOptions(roles);

				page = 1;
			}

			var users = await _usersService.FetchUsersAsync(role, keyword);
			var pagedList = users.GetPagedList(_mapper, page, pageSize);

			foreach (var item in pagedList.ViewList)
			{
				var roles = _usersService.GetRolesByUserId(item.Id);
				item.Roles = String.Join(",", roles.Select(r => r.Name));
			}

			model.PagedList = pagedList;

			return Ok(model);
		}

	}
}
