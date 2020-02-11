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
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Exceptions;
using Web.Models;
using Web.Helpers;
using Web.Helpers.ViewServices;
using ApplicationCore.Specifications;

namespace Web.Controllers
{
	[Authorize]
	public class TermsController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;

		public TermsController(ITermsService termsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
		}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var term = _termsService.GetById(id);
			if (term == null) return NotFound();


			return Ok(term.MapViewModel(_mapper));
		}
	}
}
