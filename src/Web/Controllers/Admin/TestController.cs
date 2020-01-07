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
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Web.Controllers.Admin
{
	public class ATestController : ControllerBase
	{

		private readonly IQuestionsService _questionsService;
		private readonly IRecruitsService _recruitsService;
		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;
		private readonly IMapper _mapper;

		private readonly IHostingEnvironment environment;
		private readonly AppSettings appSettings;

		public ATestController(IQuestionsService questionsService, IRecruitsService recruitsService,
			ISubjectsService subjectsService, ITermsService termsService, IMapper mapper,
			IHostingEnvironment environment, IOptions<AppSettings> appSettings)
		{
			_questionsService = questionsService;
			_recruitsService = recruitsService;
			_subjectsService = subjectsService;
			_termsService = termsService;

			_mapper = mapper;

			this.environment = environment;
			this.appSettings = appSettings.Value;
		}

		string UploadFilesPath => Path.Combine(environment.WebRootPath, appSettings.UploadPath);

		[HttpGet("test")]
		public async Task<ActionResult> Test()
		{
			//var question = _questionsService.GetById(24);
			//var allRecruits = await _recruitsService.GetAllAsync();



			//return Ok(question.MapViewModel(_mapper, allRecruits.ToList()));
			var recruit = await _recruitsService.GetByIdAsync(24);

			var allItems = await _recruitsService.GetAllAsync();

			recruit.LoadParents(allItems);

			var model = recruit.MapViewModel(_mapper);

			return Ok(UploadFilesPath);
		}



		[HttpGet("")]
		public async Task<ActionResult> Index(int recruit)
		{
			Recruit selectedRecruit = _recruitsService.GetById(recruit);
			if (selectedRecruit == null)
			{
				ModelState.AddModelError("recruit", "年度不存在");
				return BadRequest(ModelState);
			}

			if (selectedRecruit.RecruitEntityType == RecruitEntityType.Year) return Ok(new List<Question>().GetPagedList(_mapper));

			var allSubjects = await _subjectsService.FetchAsync();

			var subject = _recruitsService.FindSubject(selectedRecruit, allSubjects);

			if (subject == null)
			{
				ModelState.AddModelError("subject", "科目不存在");
				return BadRequest(ModelState);
			}

			_subjectsService.LoadSubItems(subject);

			var questions = await _questionsService.FetchByRecruitAsync(selectedRecruit, subject);
			if (questions.HasItems())
			{
				var allTerms = await _termsService.FetchAllAsync();
				foreach (var question in questions)
				{
					question.LoadTerms(allTerms);
					question.Options = question.Options.OrderByDescending(o => o.Correct).ToList();
				}
			}

			var pageList = questions.GetPagedList(_mapper);

			return Ok(pageList);
		}



	}
}
