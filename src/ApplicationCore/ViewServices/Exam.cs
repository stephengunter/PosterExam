using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Views;
using AutoMapper;

namespace ApplicationCore.ViewServices
{
	public static class ExamViewService
	{
		#region  MapViewModel
		public static ExamViewModel MapViewModel(this Exam exam, IMapper mapper) => mapper.Map<ExamViewModel>(exam);

		public static ExamViewModel MapViewModel(this Exam exam, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null)
		{
			var options = exam.Parts.SelectMany(p => p.Questions)
								 .SelectMany(q => q.Options);

			bool hasResolves = resolves.HasItems();

			foreach (var option in options)
			{
				option.LoadAttachments(attachments);
			}

			if (hasResolves)
			{

			}
			else
			{
				var examQuestions = exam.Parts.SelectMany(p => p.Questions);
				foreach (var examQuestion in examQuestions)
				{
					if(examQuestion.Question != null) examQuestion.Question.Resolves = new List<Resolve>();

				}
			}

			var model = mapper.Map<ExamViewModel>(exam);

			var optionViews = model.Parts.SelectMany(c => c.Questions)
								 .SelectMany(t => t.Options);

			foreach (var optionView in optionViews)
			{
				optionView.Correct = false;
			}

			

			return model;
		}

		public static List<ExamViewModel> MapViewModelList(this IEnumerable<Exam> exams, IMapper mapper)
			=> exams.Select(item => MapViewModel(item, mapper)).ToList();

		public static List<ExamViewModel> MapViewModelList(this IEnumerable<Exam> exams, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null)
			=> exams.Select(item => MapViewModel(item, mapper, attachments, resolves)).ToList();


		public static PagedList<Exam, ExamViewModel> GetPagedList(this IEnumerable<Exam> exams, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Exam, ExamViewModel>(exams, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}

		public static PagedList<Exam, ExamViewModel> GetPagedList(this IEnumerable<Exam> exams, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null,
			int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Exam, ExamViewModel>(exams, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, attachments, resolves);

			pageList.List = null;

			return pageList;
		}


		#endregion


		#region  MapEntity
		public static Exam MapEntity(this ExamViewModel model, IMapper mapper, string currentUserId)
		{
			var examQuestionModels = model.Parts.SelectMany(p => p.Questions);
			foreach (var examQuestionModel in examQuestionModels)
			{
				examQuestionModel.Question = null;
				examQuestionModel.Options = null;
			}

			var entity = mapper.Map<ExamViewModel, Exam>(model);

			if (model.Id == 0) entity.SetCreated(currentUserId);
			entity.SetUpdated(currentUserId);

			return entity;
		}
		#endregion

		public static IEnumerable<Exam> GetOrdered(this IEnumerable<Exam> exams)
			=> exams.OrderByDescending(item => item.LastUpdated);



	}
}
