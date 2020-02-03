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

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{
		private readonly INotesService _notesService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public ATestController(INotesService notesService, ISubjectsService subjectsService, ITermsService termsService,
			 IAttachmentsService attachmentsService, IMapper mapper)
		{
			_notesService = notesService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_attachmentsService = attachmentsService;

			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int term = 0, int subject = 0, string keyword = "")
		{
			if (!String.IsNullOrEmpty(keyword))
			{
				var notes = await GetNotesByKeywordsAsync(subject, keyword);
				var postIds = notes.Select(x => x.Id).ToList();
				var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

				var termIds = notes.Select(x => x.TermId).Distinct();
				var termViewModels = new List<TermViewModel>();
				foreach (var itemId in termIds)
				{
					termViewModels.Add(LoadTermViewModel(itemId, notes, attachments));
				}
				return Ok(termViewModels);
			}

			return Ok();
		}


		async Task<List<Note>> GetNotesByKeywordsAsync(int subject, string keyword)
		{
			var selectedSubject = _subjectsService.GetById(subject);
			var subjectIds = new List<int> { subject };
			if (selectedSubject.SubItems.HasItems()) subjectIds.AddRange(selectedSubject.GetSubIds());

			var terms = await _termsService.FetchAsync(new TermFilterBySubjectsSpecification(subjectIds));
			var termIds = terms.Select(x => x.Id).ToList();

			var notes = await _notesService.FetchAsync(new NoteTermFilterSpecification(termIds));


			var keywords = keyword.GetKeywords();
			if (keywords.HasItems()) notes = notes.FilterByKeyword(keywords);


			return notes.ToList();
		}

		TermViewModel LoadTermViewModel(int term, List<Note> noteList, List<UploadFile> attachments)
		{
			var selectedTerm = _termsService.GetById(term);
			var termIds = new List<int> { selectedTerm.Id };
			if (selectedTerm.SubItems.HasItems()) termIds.AddRange(selectedTerm.GetSubIds());

			var notes = noteList.Where(x => termIds.Contains(x.TermId));

			var noteViewList = notes.MapViewModelList(_mapper, attachments);

			var termViewModel = selectedTerm.MapViewModel(_mapper);
			termViewModel.LoadNotes(noteViewList);

			return termViewModel;
		}

	}
}
