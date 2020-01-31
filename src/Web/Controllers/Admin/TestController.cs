﻿using System;
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

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{
		private readonly IAttachmentsService _attachmentsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		public ATestController(ISubjectsService subjectsService, ITermsService termsService,
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

				//subitems
				var subjects = rootSubjects.SelectMany(p => p.SubItems);


				model.Subjects = subjects.MapViewModelList(_mapper);
				model.Subject = model.RootSubjects.FirstOrDefault();
				subject = model.Subject.Id;
			}



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


		//[HttpGet("")]
		//public async Task<ActionResult> Index(int subject)
		//{
		//	Subject selectedSubject = await _subjectsService.GetByIdAsync(subject);
		//	if (selectedSubject == null)
		//	{
		//		ModelState.AddModelError("subject", "科目不存在");
		//		return BadRequest(ModelState);
		//	}

		//	_subjectsService.LoadSubItems(selectedSubject);

		//	//全部terms
		//	int parentTermId = -1;
		//	var terms = await _termsService.FetchAsync(selectedSubject, parentTermId);
		//	if (terms.HasItems())
		//	{
		//		_termsService.LoadSubItems(terms);

		//		terms = terms.GetOrdered();
		//	}


		//	return Ok(selectedSubject.MapViewModel(_mapper));
		//}



	}
}
