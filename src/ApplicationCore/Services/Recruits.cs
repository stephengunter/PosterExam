using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services
{
	public interface IRecruitsService
	{
		Task<IEnumerable<Recruit>> FetchAsync(int parentId, bool active);
		IEnumerable<Recruit> FetchRootItems();
		Task<Recruit> GetByIdAsync(int id);
		Task<IEnumerable<Recruit>> GetAllAsync();
		Task<Recruit> CreateAsync(Recruit recruit, ICollection<Recruit> subItems = null);

		Task UpdateOrderAsync(int target, int replace, bool up);
		Task UpdateAsync(Recruit recruit);
		Task UpdateAsync(Recruit existingEntity, Recruit model, ICollection<Recruit> subItems = null);
		Task RemoveAsync(Recruit recruit);

		void LoadSubItems(Recruit entity);
		void LoadSubItems(IEnumerable<Recruit> list);
		Recruit GetById(int id);

		void LoadRecruitsText(IEnumerable<Question> list);
	}

	public class RecruitsService : BaseCategoriesService<Recruit>, IRecruitsService
	{
		private readonly IDefaultRepository<Recruit> _recruitRepository;

		public RecruitsService(IDefaultRepository<Recruit> recruitRepository)
		{
			this._recruitRepository = recruitRepository;
		}

		
		public async Task<IEnumerable<Recruit>> GetAllAsync() => await _recruitRepository.ListAsync(new RecruitFilterSpecification());

		public async Task<IEnumerable<Recruit>> FetchAsync(int parentId, bool active)
		{
			IEnumerable<Recruit> list;
			if (parentId < 0) list = await GetAllAsync();
			else list = await _recruitRepository.ListAsync(new RecruitFilterSpecification(parentId));

			return list.Where(x => x.Active == active);
		}

		public IEnumerable<Recruit> FetchRootItems() => AllRootItems(_recruitRepository.DbSet);

		public async Task<Recruit> GetByIdAsync(int id) => await _recruitRepository.GetByIdAsync(id);

		public async Task<Recruit> CreateAsync(Recruit recruit, ICollection<Recruit> subItems = null)
		{
			await _recruitRepository.AddAsync(recruit);
			if (subItems.HasItems())
			{
				foreach (var item in subItems)
				{
					item.ParentId = recruit.Id;
				}

				_recruitRepository.AddRange(subItems);
			}

			return recruit;

		}

		public async Task UpdateOrderAsync(int target, int replace, bool up)
		{
			var targetEntity = await _recruitRepository.GetByIdAsync(target);
			int targetOrder = targetEntity.Order;

			var replaceEntity = await _recruitRepository.GetByIdAsync(replace);
			int replaceOrder = replaceEntity.Order;

			

			targetEntity.Order = replaceOrder;
			replaceEntity.Order = targetOrder;

			if (targetEntity.Order == replaceEntity.Order)
			{
				if (up) replaceEntity.Order += 1;
				else targetEntity.Order += 1;
			}

			_recruitRepository.UpdateRange(new List<Recruit> { targetEntity, replaceEntity });
		}

		public async Task UpdateAsync(Recruit recruit) => await _recruitRepository.UpdateAsync(recruit);

		public async Task UpdateAsync(Recruit existingEntity, Recruit model, ICollection<Recruit> subItems = null)
		{
			int recruitId = model.Id;
			

			await _recruitRepository.UpdateAsync(existingEntity, model);
			if (subItems.HasItems())
			{
				foreach (var item in subItems)
				{
					item.ParentId = recruitId;
				}
			}

			var existingSubItems = _recruitRepository.DbSet.Where(x => x.ParentId == existingEntity.Id).ToList();

			SyncSubItems(existingSubItems, subItems);

		}
		

		public async Task RemoveAsync(Recruit recruit)
		{
			recruit.Removed = true;
			await _recruitRepository.UpdateAsync(recruit);
		}

		public Recruit GetById(int id)
		{
			var recruit = _recruitRepository.GetById(id);
			if (recruit == null) return null;

			LoadSubItems(recruit);

			return recruit;
		}

		public void LoadRecruitsText(IEnumerable<Question> questions)
		{
			var rootItems =  AllRootItems(_recruitRepository.DbSet);

			foreach (var question in questions)
			{
				var parentList = question.Recruits.Select(item => item.GetParent(rootItems));
				question.SetRecruitsText(parentList.ToList());
			}
		}

		public void LoadSubItems(IEnumerable<Recruit> list)
		{
			if (list.IsNullOrEmpty()) return;

			var subItems = AllSubItems(_recruitRepository.DbSet);


			foreach (var entity in list)
			{
				entity.LoadSubItems(subItems.ToList());
			}
		}

		public void LoadSubItems(Recruit entity)
		{
			var subItems = AllSubItems(_recruitRepository.DbSet);

			entity.LoadSubItems(subItems.ToList());
		}

		void SyncSubItems(ICollection<Recruit> existingList, ICollection<Recruit> latestList)
		{
			if (latestList.IsNullOrEmpty()) latestList = new List<Recruit>();

			foreach (var existingItem in existingList)
			{
				if (!latestList.Any(item => item.Id == existingItem.Id))
				{
					existingItem.ParentId = 0;
					existingItem.Removed = true;
				}
			}

			foreach (var latestItem in latestList)
			{
				var existingItem = existingList.Where(item => item.Id == latestItem.Id).FirstOrDefault();

				if (existingItem != null) _recruitRepository.DbContext.Entry(existingItem).CurrentValues.SetValues(latestItem);
				else _recruitRepository.DbSet.Add(latestItem);

			}

			_recruitRepository.DbContext.SaveChanges();

		}
	}
}
