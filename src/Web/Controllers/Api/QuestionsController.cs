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
		public async Task<ActionResult> Index(int term)
		{
			var selectedTerm = _termsService.GetById(term);
			if (selectedTerm == null) return NotFound();

			var subject = _subjectsService.GetById(selectedTerm.SubjectId);

			var termIds = selectedTerm.GetSubIds();
			termIds.Add(selectedTerm.Id);

			var questions = (await _questionsService.FetchAsync(subject, termIds)).ToList();

			var allRecruits = await _recruitsService.GetAllAsync();
			List<Term> allTerms = null;

			var types = new List<PostType> { PostType.Option };
			var attachments = await _attachmentsService.FetchByTypesAsync(types);


			var models = questions.MapViewModelList(_mapper, allRecruits.ToList(), attachments.ToList(), allTerms);

			var sources = models.SelectMany(q => q.Resolves).SelectMany(r => r.Sources);
			foreach (var item in sources)
			{
				item.MapContent(_notesService, _termsService);
			}


			return Ok(models);
		}
	}
}
