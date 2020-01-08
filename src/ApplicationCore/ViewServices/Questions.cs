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
	public static class QuestionsViewService
	{
		#region MapViewModel

		public static QuestionViewModel MapViewModel(this Question question, IMapper mapper)
			=> mapper.Map<QuestionViewModel>(question);

		public static QuestionViewModel MapViewModel(this Question question, IMapper mapper, ICollection<Recruit> allRecruits)
		{
			LoadQuestionData(question, allRecruits);
			if (question.Recruits.HasItems())
			{
				var model = mapper.Map<QuestionViewModel>(question);
				model.Recruits = question.Recruits.MapViewModelList(mapper);

				return model;
			}
			else return mapper.Map<QuestionViewModel>(question);

		}

		static void LoadQuestionData(Question question, ICollection<Recruit> allRecruits, ICollection<UploadFile> attachmentsList = null)
		{
			if (allRecruits.HasItems())
			{
				if (question.Recruits.HasItems())
				{
					foreach (var item in question.Recruits)
					{
						item.LoadParents(allRecruits);
					}
				}
			}

			if (attachmentsList.HasItems())
			{
				foreach (var option in question.Options)
				{
					option.LoadAttachments(attachmentsList);
				}
			}

		}

		public static QuestionViewModel MapViewModel(this Question question, IMapper mapper, ICollection<Recruit> allRecruits, ICollection<UploadFile> attachmentsList)
		{
			LoadQuestionData(question, allRecruits, attachmentsList);

			if (question.Recruits.HasItems())
			{
				var model = mapper.Map<QuestionViewModel>(question);
				model.Recruits = question.Recruits.MapViewModelList(mapper);

				return model;
			}
			else return mapper.Map<QuestionViewModel>(question);
		}


		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper)
			=> questions.Select(item => MapViewModel(item, mapper)).ToList();

		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit> rootRecruits)
			=> questions.Select(item => MapViewModel(item, mapper, rootRecruits)).ToList();

		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit> rootRecruits, ICollection<UploadFile> attachments)
			=> questions.Select(item => MapViewModel(item, mapper, rootRecruits, attachments)).ToList();


		#endregion

		

		public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}

		public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit> allRecruits, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, allRecruits);

			pageList.List = null;

			return pageList;
		}

		public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper,
			ICollection<Recruit> allRecruits, ICollection<UploadFile> attachments, int page = 1, int pageSize = 999)
		{
			var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper, allRecruits, attachments);

			pageList.List = null;

			return pageList;
		}



		#region MapEntity
		public static Question MapEntity(this QuestionViewModel model, IMapper mapper, string currentUserId)
		{
			var entity = mapper.Map<QuestionViewModel, Question>(model);

			if (model.Id == 0)
			{
				entity.SetCreated(currentUserId);
			}
			else
			{
				foreach (var option in entity.Options)
				{
					option.QuestionId = entity.Id;
				}
				entity.SetUpdated(currentUserId);
			}

			return entity;
		}

		#endregion

	}
}
