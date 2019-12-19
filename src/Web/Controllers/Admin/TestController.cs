using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Admin
{
	public class TestController : BaseAdminController
	{
		
		private readonly DefaultContext _context;

		public TestController(DefaultContext context)
		{
			_context = context;
		}

		[HttpGet("")]
		public ActionResult Index(int subject, int parent = 0)
		{
			var categories = _context.Categories.ToList();
			return Ok(categories);
		}


	}
}
