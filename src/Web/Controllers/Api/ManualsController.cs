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
		public async Task<ActionResult> Index()
		{
			var manuals = await _manualsService.FetchAsync();

			manuals = manuals.GetOrdered().ToList();

			return Ok(manuals);
		}


		[HttpGet("{id}/{user?}")]
		public async Task<ActionResult> Details(int id, string user = "")
		{
			var manual = await _manualsService.GetByIdAsync(id);
			if (manual == null) return NotFound();

			if (!manual.Active)
			{
				var existingUser = await _usersService.FindUserByIdAsync(user);
				if(existingUser == null) return NotFound();

				bool isAdmin = await _usersService.IsAdminAsync(existingUser);
				if(!isAdmin) return NotFound();
			}

			return Ok(manual.MapViewModel(_mapper));
		}



	}

	
}
