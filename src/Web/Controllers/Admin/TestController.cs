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
using Web.Models;
using ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Infrastructure.Entities;

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{
		private readonly DefaultContext _context;

		public ATestController(DefaultContext context)
		{
			_context = context;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index()
		{
			var a = new List<int> { 1, 2, 3 };
			var b = new List<int> { 1,2,3,4, 5 };
			return Ok(a.Except(b));
		}

	}
}
