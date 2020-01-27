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

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{
		private readonly IExamsService _examsService;
		private readonly IRecruitsService _recruitsService;
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IMapper _mapper;

		public ATestController(IQuestionsService questionsService, IRecruitsService recruitsService, IExamsService examsService,
			IAttachmentsService attachmentsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_examsService = examsService;
			_attachmentsService = attachmentsService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int id)
		{
			var exam = _examsService.GetById(id);

			var qids = exam.Parts.SelectMany(p => p.Questions).Select(x => x.QuestionId);

			






			return Ok(qids);
		}


	}
}
