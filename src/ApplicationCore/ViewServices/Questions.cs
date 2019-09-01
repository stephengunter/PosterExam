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
		{
			return mapper.Map<QuestionViewModel>(question);
		}

		public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper)
		{
			return questions.Select(item => MapViewModel(item, mapper)).ToList();
		}

		public static Question MapEntity(this QuestionViewModel model, IMapper mapper, Question question = null)
		{
			if (question == null) question = new Question();
			
			return mapper.Map<QuestionViewModel, Question>(model, question);
		}
	}
}
