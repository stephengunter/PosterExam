﻿using System;
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
	public static class RecruitsViewService
	{
		public static RecruitViewModel MapViewModel(this Recruit recruit, IMapper mapper)
		{ 
			var model = mapper.Map<RecruitViewModel>(recruit);
			model.DateText = recruit.Date.ToDateString();

			if (recruit.SubItems.HasItems())
			{
				var subjectIds = new List<int>();
				foreach (var item in recruit.SubItems)
				{
					subjectIds.AddRange(item.SubjectIds);
				}
				model.SubjectIds = subjectIds;
			}
			return model;
		}

		public static List<RecruitViewModel> MapViewModelList(this IEnumerable<Recruit> recruits, IMapper mapper) => recruits.Select(item => MapViewModel(item, mapper)).ToList();

		public static Recruit MapEntity(this RecruitViewModel model, IMapper mapper, string currentUserId)
		{ 
			var entity = mapper.Map<RecruitViewModel, Recruit>(model);
			entity.Date = model.DateText.ToDatetimeOrNull();

			if (model.Id == 0)
			{
				entity.SetCreated(currentUserId);
				foreach (var item in entity.SubItems)
				{
					item.SetCreated(currentUserId);
				}
			}
			else
			{
				entity.SetUpdated(currentUserId);
				foreach (var item in entity.SubItems)
				{
					item.SetUpdated(currentUserId);
				}
			} 

			return entity;
		}

		public static IEnumerable<Recruit> GetOrdered(this IEnumerable<Recruit> recruits)
			=> recruits.OrderByDescending(item => item.Year).ThenBy(item => item.Order);
	}
}
