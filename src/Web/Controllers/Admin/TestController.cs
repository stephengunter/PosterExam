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
			var connectionString = _context.Database.GetDbConnection().ConnectionString;
			var builder = new SqlConnectionStringBuilder(connectionString);
			return Ok(builder.InitialCatalog);
		}

	}
}
