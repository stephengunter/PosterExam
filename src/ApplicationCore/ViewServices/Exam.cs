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
		public static ExamViewModel MapViewModel(this Exam exam, IMapper mapper, ICollection<UploadFile> attachments, ICollection<Resolve> resolves = null)
		{
			var options = exam.Parts.SelectMany(c => c.Questions)
								 .SelectMany(t => t.Options);

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
				var questions = exam.Parts.SelectMany(c => c.Questions);
				foreach (var question in questions)
				{
					question.Question.Resolves = new List<Resolve>();
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

		
	}
}
