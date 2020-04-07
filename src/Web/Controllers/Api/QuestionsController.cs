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
using ApplicationCore.Paging;

namespace Web.Controllers
{
	//[Authorize]
	public class QuestionsController : BaseController
	{
		private readonly IMapper _mapper;
		private readonly ITermsService _termsService;
		private readonly INotesService _notesService;
		private readonly ISubjectsService _subjectsService;
		private readonly IQuestionsService _questionsService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly IRecruitsService _recruitsService;

		public QuestionsController(ITermsService termsService, ISubjectsService subjectsService, INotesService notesService,
			IQuestionsService questionsService, IRecruitsService recruitsService,
			IAttachmentsService attachmentsService, IMapper mapper)
		{
			_mapper = mapper;
			_termsService = termsService;
			_notesService = notesService;
			_questionsService = questionsService;
			_subjectsService = subjectsService;
			_attachmentsService = attachmentsService;
			_recruitsService = recruitsService;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int term = 0, int subject = 0)
		{
			if (term > 0)
			{
				var selectedTerm = _termsService.GetById(term);
				if (selectedTerm == null) return NotFound();

				var qIds = selectedTerm.GetQuestionIds();

				if (qIds.HasItems()) qIds = qIds.Distinct().ToList();

				var questions = await _questionsService.FetchByIdsAsync(qIds);

				var viewList = await LoadQuestionViewsAsync(questions);

				return Ok(viewList);
			}
			else if (subject > 0)
			{
				Subject selectedSubject = _subjectsService.GetById(subject);
				if (selectedSubject == null) return NotFound();

				var questions = (await _questionsService.FetchAsync(selectedSubject)).ToList();
				var questionViews = await LoadQuestionViewsAsync(questions);
				return Ok(questionViews);
			}

			ModelState.AddModelError("params", "錯誤的查詢參數");
			return BadRequest(ModelState);
		}


		async Task<List<QuestionViewModel>> LoadQuestionViewsAsync(IEnumerable<Question> questions)
		{
			var allRecruits = await _recruitsService.GetAllAsync();
			List<Term> allTerms = null;

			var types = new List<PostType> { PostType.Option };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);

			var viewList = questions.MapViewModelList(_mapper, allRecruits.ToList(), attachments.ToList(), allTerms);

			var sources = viewList.SelectMany(q => q.Resolves).SelectMany(r => r.Sources);
			foreach (var item in sources)
			{
				item.MapContent(_notesService, _termsService);
			}


			return viewList;
		}
	}
}
