using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api
{
	public class ManualsController : BaseApiController
	{
		private readonly IManualsService _manualsService;
		private readonly IUsersService _usersService;
		private readonly IMapper _mapper;

		public ManualsController(IManualsService manualsService, IUsersService usersService, IMapper mapper)
		{
			_manualsService = manualsService;
			_usersService = usersService;
			_mapper = mapper;
		}
	

		[HttpGet("")]
		public async Task<ActionResult> Index(int id = 0, int feature = 0, string user = "")
		{
			if (id > 0 || feature > 0) return await Details(id, feature, user);

			
			var manuals = await _manualsService.FetchAsync();

			manuals = manuals.GetOrdered().ToList();

			return Ok(manuals);
		}



		async Task<ActionResult> Details(int id = 0, int feature = 0, string user = "")
		{
			bool subItems = true;
			var manual = await _manualsService.GetByIdAsync(id, subItems);
			if (manual == null) return NotFound();

			//if (!manual.Active)
			//{
			//	var existingUser = await _usersService.FindUserByIdAsync(user);
			//	if(existingUser == null) return NotFound();

			//	bool isAdmin = await _usersService.IsAdminAsync(existingUser);
			//	if(!isAdmin) return NotFound();
			//}

			return Ok(manual.MapViewModel(_mapper));
		}



	}

	
}
