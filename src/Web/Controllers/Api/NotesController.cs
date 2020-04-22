using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.ViewServices;
using Microsoft.AspNetCore.Authorization;
using Web.Helpers;
using Web.Controllers;

namespace Web.Controllers.Api
{
	[Authorize]
	public class NotesController : BaseApiController
	{
		private readonly IDataService _dataService;

		private readonly INotesService _notesService;
		private readonly IMapper _mapper;

		public NotesController(IDataService dataService, INotesService notesService, 
			 IMapper mapper)
		{
			_dataService = dataService;

			_notesService = notesService;

			_mapper = mapper;
		}

		[HttpGet("categories")]
		public ActionResult Categories()
		{
			var categories = _dataService.FetchNoteCategories();

			return Ok(categories);
		}

		[HttpGet("")]
		public ActionResult Index(int term = 0, int subject = 0, string keyword = "")
		{
			if (term > 0)
			{
				var termViewModel = _dataService.FindTermNotesByTerm(term);
				if (termViewModel == null) return NotFound();

				if (termViewModel.SubItems.HasItems()) return Ok(termViewModel.SubItems);
				else return Ok(new List<TermViewModel> { termViewModel });
			}
			else if (subject > 0)
			{
				var keywords = keyword.GetKeywords();
				var termViewList = _dataService.FetchTermNotesBySubject(subject);

				if (keywords.IsNullOrEmpty()) return Ok(termViewList);


				var termsHasKeywords = FilterByKeywords(termViewList, keywords);

				var noteIds = FetchNoteIdsByKeywords(termViewList, keywords);

				var resultList = new List<TermViewModel>();
				if (termsHasKeywords.HasItems()) resultList.AddRange(termsHasKeywords);


				var terms = termViewList.SelectMany(x => x.SubItems).ToList();
				if (terms.IsNullOrEmpty()) terms = termViewList.ToList();

				foreach (var termView in terms)
				{
					termView.Notes = termView.Notes.Where(x => noteIds.Contains(x.Id)).ToList();
					if (termView.Notes.HasItems())
					{
						if (resultList.FirstOrDefault(x => x.Id == termView.Id) == null)
						{
							resultList.Add(termView);
						}
					}
				}

				return Ok(resultList);
			}

			ModelState.AddModelError("params", "錯誤的查詢參數");
			return BadRequest(ModelState);
		}

		List<TermViewModel> FilterByKeywords(IEnumerable<TermViewModel> termViewList, IList<string> keywords)
		{
			var terms = termViewList.SelectMany(x => x.SubItems).ToList();
			if (terms.HasItems())
			{
				return terms.Where(item => keywords.Any(item.Text.Contains)).ToList();
			}
			else
			{

				return termViewList.Where(item => keywords.Any(item.Text.Contains)).ToList();
			}
		}

		List<int> FetchNoteIdsByKeywords(IEnumerable<TermViewModel> termViewList, IList<string> keywords)
		{
			var terms = termViewList.SelectMany(x => x.SubItems).ToList();
			if (terms.HasItems())
			{
				return terms.SelectMany(x => x.Notes).Where(item => keywords.Any(item.HasKeyword)).Select(x => x.Id).ToList();
			}
			else
			{
				return termViewList.SelectMany(x => x.Notes).Where(item => keywords.Any(item.HasKeyword)).Select(x => x.Id).ToList();
			}


		}

		//[HttpGet("old_index")]
		//public async Task<ActionResult> OldIndex(int mode, int term = 0, int subject = 0, string keyword = "")
		//{
		//	if (term > 0)
		//	{
		//		var selectedTerm = _termsService.GetById(term);
		//		var termViewModel = await LoadTermViewModelAsync(mode, selectedTerm);


		//		if (termViewModel.SubItems.HasItems()) return Ok(termViewModel.SubItems);
		//		else return Ok(new List<TermViewModel> { termViewModel });
		//	}
		//	else if (subject > 0)
		//	{
		//		var keywords = keyword.GetKeywords();
		//		Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
		//		int parent = -1;
		//		//科目底下所有條文
		//		var terms = (await _termsService.FetchAsync(selectedSubject, parent)).Where(x => !x.ChapterTitle);
		//		var termIds = terms.Select(x => x.Id).ToList();

		//		if (terms.HasItems())
		//		{
		//			_termsService.LoadSubItems(terms);
					
		//			if (keywords.HasItems()) terms = terms.FilterByKeyword(keywords);
		//			terms = terms.GetOrdered();
		//		}

		//		var termViewModelList = new List<TermViewModel>();
		//		foreach (var item in terms)
		//		{
		//			var termViewModel = await LoadTermViewModelAsync(mode, item);
		//			termViewModelList.Add(termViewModel);
		//		}


		//		if (keywords.HasItems())
		//		{
		//			var notes = await _notesService.FetchAsync(termIds);
		//			notes = notes.FilterByKeyword(keywords);

		//			if (notes.HasItems())
		//			{
		//				foreach (int termId in notes.Select(x => x.TermId).Distinct())
		//				{
		//					var exist = termViewModelList.FirstOrDefault(x => x.Id == termId);
		//					if (exist == null)
		//					{
		//						var selectedTerm = _termsService.GetById(termId);
		//						var noteInTerms = notes.Where(x => x.TermId == termId);

		//						var termViewModel = await LoadTermViewModelAsync(mode, selectedTerm);
		//						termViewModelList.Add(termViewModel);
		//					}
		//				}

		//				termViewModelList = termViewModelList.OrderBy(item => item.Order).ToList();

		//			}

					
		//		}

				
		//		return Ok(termViewModelList);
		//	}

		//	ModelState.AddModelError("params", "錯誤的查詢參數");
		//	return BadRequest(ModelState);
		//}

		//async Task<TermViewModel> LoadTermViewModelAsync(int mode, Term term)
		//{
		//	var termIds = new List<int>() { term.Id };
		//	if (term.SubItems.HasItems()) termIds.AddRange(term.GetSubIds());
		//	var notes = await _notesService.FetchAsync(termIds);

		//	if (mode > 0) notes = notes.Where(x => x.Important);


		//	var postIds = notes.Select(x => x.Id).ToList();
		//	var attachments = (await _attachmentsService.FetchAsync(PostType.Note, postIds)).ToList();

		//	var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

		//	var termViewModel = term.MapViewModel(_mapper);
		//	termViewModel.Subject = null;
		//	if (termViewModel.SubItems.HasItems())
		//	{
		//		foreach (var item in termViewModel.SubItems)
		//		{
		//			item.Subject = null;
		//		}
		//	}
		//	termViewModel.LoadNotes(noteViewList);

		//	return termViewModel;
		//}

		[HttpGet("{id}")]
		public ActionResult Details(int id)
		{
			var note = _notesService.GetById(id);
			if (note == null) return NotFound();

			return Ok(note.MapViewModel(_mapper));
		}



	}

	
}
