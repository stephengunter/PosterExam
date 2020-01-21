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

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{
		private readonly IExamsService _examsService;
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IMapper _mapper;

		public ATestController(IQuestionsService questionsService, IExamsService examsService,
			IAttachmentsService attachmentsService, IMapper mapper)
		{
			_questionsService = questionsService;
			_examsService = examsService;
			_attachmentsService = attachmentsService;
			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int id)
		{
			var exam = _examsService.GetById(id);
			var alloptions = await _questionsService.FetchAllOptionsAsync();

			var examQuestions = exam.Parts.SelectMany(p => p.Questions);
			foreach (var item in examQuestions) item.LoadOptions();

			var types = new List<PostType> { PostType.Option, PostType.Resolve };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			return Ok(exam.MapViewModel(_mapper, attachments.ToList()));
		}


	}
}
