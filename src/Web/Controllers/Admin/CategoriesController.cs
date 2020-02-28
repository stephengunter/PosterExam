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
using Web.Models;
using ApplicationCore.Specifications;
using Web.Helpers;

namespace Web.Controllers.Admin
{
	public class CategoriesController : BaseAdminController
	{
		private readonly INotesService _notesService;
		private readonly IAttachmentsService _attachmentsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public CategoriesController(INotesService notesService, ISubjectsService subjectsService, ITermsService termsService,
			 IAttachmentsService attachmentsService, IMapper mapper)
		{
			_notesService = notesService;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_attachmentsService = attachmentsService;

			_mapper = mapper;
		}


		[HttpGet("")]
		public async Task<ActionResult> Index(int type = 0)
		{

			//var allSubjects = await _subjectsService.FetchAsync();

			//var rootSubjects = allSubjects.Where(x => x.ParentId < 1).GetOrdered();

			//var categories = rootSubjects.Select(item => item.MapNoteCategoryViewModel()).ToList();
			//foreach (var root in categories)
			//{
			//	int parentId = root.Id;
			//	var subjects = allSubjects.Where(x => x.ParentId == parentId).GetOrdered();
			//	root.SubItems = subjects.Select(item => item.MapNoteCategoryViewModel(parentId)).ToList();
			//}

			//var subjectCategories = categories.SelectMany(x => x.SubItems);

			//var spec = new TermFilterBySubjectsSpecification(subjectCategories.Select(item => item.Id).ToList(), 0);
			//var termList = (await _termsService.FetchAsync(spec));

			//foreach (var subjectCategory in subjectCategories)
			//{
			//	var terms = termList.Where(item => item.SubjectId == subjectCategory.Id && item.ChapterTitle == true).GetOrdered();
			//	subjectCategory.SubItems = terms.Select(item => item.MapNoteCategoryViewModel()).ToList();
			//}

			//return Ok(categories);



			var allSubjects = await _subjectsService.FetchAsync();
			var allTerms = await _termsService.FetchAllAsync();

			var rootSubjects = allSubjects.Where(x => x.ParentId < 1).GetOrdered();

			var categories = rootSubjects.Select(item => item.MapNoteCategoryViewModel()).ToList();
			foreach (var root in categories)
			{
				int parentId = root.Id;
				var subjects = allSubjects.Where(x => x.ParentId == parentId).GetOrdered();
				root.SubItems = subjects.Select(item => item.MapNoteCategoryViewModel(parentId)).ToList();
			}

			var subjectCategories = categories.SelectMany(x => x.SubItems);

			foreach (var subjectCategory in subjectCategories)
			{
				var terms = allTerms.Where(item => item.SubjectId == subjectCategory.Id && item.ParentId == 0).GetOrdered();

				if (type > 0) foreach (var item in terms) item.LoadSubItems(allTerms);

				subjectCategory.SubItems = terms.Select(item => item.MapNoteCategoryViewModel()).ToList();
			}

			return Ok(categories);
		}

	}
}
