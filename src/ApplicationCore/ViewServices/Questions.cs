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
		public static QuestionViewModel MapViewModel(this Question question, IMapper mapper)
			=> mapper.Map<QuestionViewModel>(question);

		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper)
			=> questions.Select(item => MapViewModel(item, mapper)).ToList();

		public static Question MapEntity(this QuestionViewModel model, IMapper mapper)
			=> mapper.Map<QuestionViewModel, Question>(model);

		public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, int page, int pageSize)
		{
			var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

			pageList.ViewList = pageList.List.MapViewModelList(mapper);

			pageList.List = null;

			return pageList;
		}
	}
}
