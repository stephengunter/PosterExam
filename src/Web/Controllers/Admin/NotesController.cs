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

namespace Web.Controllers.Admin
{
	public class NotesController : BaseAdminController
	{
		private readonly IAttachmentsService _attachmentsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public NotesController(ISubjectsService subjectsService, ITermsService termsService,
			 IAttachmentsService attachmentsService, IMapper mapper)
		{
			_subjectsService = subjectsService;
			_termsService = termsService;
			_attachmentsService = attachmentsService;

			_mapper = mapper;
		}

		[HttpGet("")]
		public async Task<ActionResult> Index(int subject = 0)
		{
			var model = new NoteAdminViewModel();
			if (subject < 1) //初次載入頁面
			{
				int parentSubjectId = 0;
				var rootSubjects = await _subjectsService.FetchAsync(parentSubjectId);
				rootSubjects = rootSubjects.GetOrdered();

				_subjectsService.LoadSubItems(rootSubjects);

				model.RootSubjects = rootSubjects.MapViewModelList(_mapper);
			
				var subjectSubitems = rootSubjects.SelectMany(p => p.SubItems);
				model.Subjects = subjectSubitems.MapViewModelList(_mapper);

				foreach (var subjectModel in model.Subjects)
				{
					int parentTermId = 0;
					var terms = await _termsService.FetchAsync(subjectSubitems.FirstOrDefault(x => x.Id == subjectModel.Id), parentTermId);
					if (terms.HasItems())
					{
						_termsService.LoadSubItems(terms);
						terms = terms.GetOrdered();
					}

					model.Terms.AddRange(terms.MapViewModelList(_mapper));
				}

				model.Subject = model.RootSubjects.FirstOrDefault();
				subject = model.Subject.Id;



			}

		//subject: subjectId, parent: 0

			//var subjects = await _subjectsService.FetchAsync(parent);
			//subjects = subjects.GetOrdered();

			//if (subItems)
			//{
			//	_subjectsService.LoadSubItems(subjects);
			//	foreach (var item in subjects)
			//	{
			//		item.GetSubIds();
			//	}
			//}

			return Ok(model);
		}



	}
}
