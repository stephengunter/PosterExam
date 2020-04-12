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

namespace Web.Controllers.Admin
{
	public class ExceptionsController : BaseAdminController
	{
		private readonly ApplicationCore.Logging.ILogger _logger;

		public ExceptionsController(ApplicationCore.Logging.ILogger logger)
		{
			_logger = logger;
		}

		[HttpGet("")]
		public ActionResult Index(int page = 1, int pageSize = 10)
		{
			var records = _logger.FetchAllExceptions();

			var model = records.GetPagedList(page, pageSize);


			return Ok(model);
		}

	}
}
