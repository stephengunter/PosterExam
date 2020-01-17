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
			var options = exam.ExamParts.SelectMany(c => c.ExamQuestions)
								 .SelectMany(t => t.Options);

			bool hasResolves = resolves.HasItems();

			foreach (var option in options)
			{
				option.LoadAttachments(attachments);
			}

			var model = mapper.Map<ExamViewModel>(exam);
			var optionViews = model.ExamParts.SelectMany(c => c.ExamQuestions)
								 .SelectMany(t => t.Options);

			foreach (var optionView in optionViews)
			{
				optionView.Correct = false;
			}

			if (resolves.HasItems())
			{

			}
			else
			{ 
				
			}

			return model;
		}

		
	}
}
