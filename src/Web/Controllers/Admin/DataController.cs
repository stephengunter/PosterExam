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
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;
using Infrastructure.Entities;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers.Admin
{
	public class DataController : BaseAdminController
	{
		private readonly AdminSettings _adminSettings;
		private readonly IDataService _dataService;
		private readonly RootSubjectSettings _rootSubjectSettings;

		private readonly ISubjectsService _subjectsService;
		private readonly ITermsService _termsService;

		private readonly IMapper _mapper;

		public DataController(IOptions<RootSubjectSettings> rootSubjectSettings, IOptions<AdminSettings> adminSettings,
			IDataService dataService, ISubjectsService subjectsService, ITermsService termsService, IMapper mapper)
		{
			_adminSettings = adminSettings.Value;
			_rootSubjectSettings = rootSubjectSettings.Value;
			_subjectsService = subjectsService;
			_termsService = termsService;
			_dataService = dataService;

			_mapper = mapper;
		}


		//儲存每個Subject, Term 底下的QuestionId(精選試題)
		[HttpPost("subject-questions")]
		public async Task<ActionResult> StoreSubjectQuestions(AdminRequest model)
		{
			ValidateRequest(model, _adminSettings);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//專業科目(1)：臺灣自然及人文地理
			var firstRootSubject = _subjectsService.GetById(_rootSubjectSettings.FirstId);
			await SaveSubjectQuestionsAsync(firstRootSubject);


			//專業科目(2)：郵政法規大意及交通安全常識
			var secondRootSubject = _subjectsService.GetById(_rootSubjectSettings.SecondId);
			await SaveSubjectQuestionsAsync(secondRootSubject);


			return Ok();
		}

		async Task SaveSubjectQuestionsAsync(Subject rootSubject)
		{
			var subjects = rootSubject.SubItems;

			var models = new List<SubjectQuestionsViewModel>();
			foreach (var subject in subjects)
			{
				int parentId = 0;
				var terms = await _termsService.FetchAsync(subject, parentId);
				_termsService.LoadSubItems(terms);

				var subjectQuestionsModel = new SubjectQuestionsViewModel { SubjectId = subject.Id };
				foreach (var term in terms)
				{
					var termQuestionsModel = new TermQuestionsViewModel
					{
						TermId = term.Id,
						QuestionIds = term.QIds.SplitToIds()
					};

					if (term.SubItems.HasItems())
					{
						termQuestionsModel.SubItems = term.SubItems.Select(subItem => new TermQuestionsViewModel
						{
							TermId = subItem.Id,
							QuestionIds = subItem.QIds.SplitToIds()
						}).ToList();
					}

					subjectQuestionsModel.TermQuestions.Add(termQuestionsModel);

				}

				models.Add(subjectQuestionsModel);
			}

			_dataService.SaveSubjectQuestions(rootSubject.Id, models);
		}

	}
}
