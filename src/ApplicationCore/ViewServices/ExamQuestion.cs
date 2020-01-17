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
	public static class ExamQuestionViewService
	{
		public static ExamQuestionViewModel MapViewModel(this ExamQuestion examQuestion, IMapper mapper)
		{
			var model = mapper.Map<ExamQuestionViewModel>(examQuestion);
			model.Order = 65; 
			return model;
		}

	}
}
