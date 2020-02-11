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
	public class NotesController : BaseController
	{
		private readonly INotesService _notesService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public NotesController(INotesService notesService, ISubjectsService subjectsService, ITermsService termsService,
			 IAttachmentsService attachmentsService, IMapper mapper)
		{
			_notesService = notesService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_attachmentsService = attachmentsService;

			_mapper = mapper;
		}

		[HttpGet("categories")]
		public async Task<ActionResult> Categories()
		{
			var allSubjects = await _subjectsService.FetchAsync();

			var rootSubjects = allSubjects.Where(x => x.ParentId < 1).GetOrdered();

			var categories = rootSubjects.Select(item => item.MapNoteCategoryViewModel()).ToList();
			foreach (var root in categories)
			{
				int parentId = root.Id;
				var subjects = allSubjects.Where(x => x.ParentId == parentId);
				root.SubItems = subjects.Select(item => item.MapNoteCategoryViewModel(parentId)).ToList();
			}

			var subjectCategories = categories.SelectMany(x => x.SubItems);

			var spec = new TermFilterBySubjectsSpecification(subjectCategories.Select(item => item.Id).ToList(), 0);
			var termList = (await _termsService.FetchAsync(spec));

			foreach (var subjectCategory in subjectCategories)
			{
				var terms = termList.Where(item => item.SubjectId == subjectCategory.Id && item.ChapterTitle == true);
				subjectCategory.SubItems = terms.Select(item => item.MapNoteCategoryViewModel()).ToList();
			}

			return Ok(categories);
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int term = 0, int subject = 0, string keyword = "")
		{
			if (term > 0)
			{
				var selectedTerm = _termsService.GetById(term);
				var termViewModel = await LoadTermViewModelAsync(selectedTerm);


				if (termViewModel.SubItems.HasItems()) return Ok(termViewModel.SubItems);
				else return Ok(new List<TermViewModel> { termViewModel });
			}
			else if (subject > 0)
			{
				var keywords = keyword.GetKeywords();
				Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
				int parent = -1;
				//科目底下所有條文
				var terms = (await _termsService.FetchAsync(selectedSubject, parent)).Where(x => !x.ChapterTitle);
				var termIds = terms.Select(x => x.Id).ToList();

				if (terms.HasItems())
				{
					_termsService.LoadSubItems(terms);
					
					if (keywords.HasItems()) terms = terms.FilterByKeyword(keywords);
					terms = terms.GetOrdered();
				}

				var termViewModelList = new List<TermViewModel>();
				foreach (var item in terms)
				{
					var termViewModel = await LoadTermViewModelAsync(item);
					termViewModelList.Add(termViewModel);
				}


				if (keywords.HasItems())
				{
					var notes = await _notesService.FetchAsync(termIds);
					notes = notes.FilterByKeyword(keywords);

					if (notes.HasItems())
					{
						foreach (int termId in notes.Select(x => x.TermId).Distinct())
						{
							var exist = termViewModelList.FirstOrDefault(x => x.Id == termId);
							if (exist == null)
							{
								var selectedTerm = _termsService.GetById(termId);
								var noteInTerms = notes.Where(x => x.TermId == termId);

								var termViewModel = await LoadTermViewModelAsync(selectedTerm);
								termViewModelList.Add(termViewModel);
							}
						}

						termViewModelList = termViewModelList.OrderBy(item => item.Order).ToList();

					}

					
				}

				
				return Ok(termViewModelList);
			}

			ModelState.AddModelError("params", "錯誤的查詢參數");
			return BadRequest(ModelState);
		}

		async Task<TermViewModel> LoadTermViewModelAsync(Term term, IEnumerable<Note> notes = null)
		{
			if (notes == null)
			{
				var termIds = new List<int>() { term.Id };
				if (term.SubItems.HasItems()) termIds.AddRange(term.GetSubIds());
				notes = await _notesService.FetchAsync(termIds);
			}

			var postIds = notes.Select(x => x.Id).ToList();
			var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

			var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

			var termViewModel = term.MapViewModel(_mapper);
			termViewModel.LoadNotes(noteViewList);

			return termViewModel;
		}

	}
}
