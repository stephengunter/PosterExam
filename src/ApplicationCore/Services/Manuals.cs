using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApplicationCore.Helpers;
using Infrastructure.DataAccess;

namespace ApplicationCore.Services
{
	public interface IManualsService
	{
		Task<IEnumerable<Manual>> FetchAsync(bool active = true);
		Task<Manual> GetByIdAsync(int id);
		Task<Manual> CreateAsync(Manual manual);
		Task UpdateAsync(Manual manual);
		Task UpdateAsync(Manual existingEntity, Manual model);
		Task RemoveAsync(Manual manual);

		Manual GetById(int id);

		Feature GetFeatureById(int id);
		Task<Feature> GetFeatureByIdAsync(int id);
		Task<Feature> CreateFeatureAsync(Feature feature);
		Task UpdateAsync(Feature existingEntity, Feature feature);
	}

	public class ManualsService : BaseCategoriesService<Manual>, IManualsService
	{
		private readonly IDefaultRepository<Manual> _manualRepository;
		private readonly IDefaultRepository<Feature> _featureRepository;

		public ManualsService(IDefaultRepository<Manual> manualRepository, IDefaultRepository<Feature> featureRepository)
		{
			_manualRepository = manualRepository;
			_featureRepository = featureRepository;
		}

		public async Task<IEnumerable<Manual>> FetchAsync(bool active = true)
		{
			var allItems = await FetchAllAsync();
			var rootItems = allItems.Where(x => x.IsRootItem);

			rootItems = rootItems.Where(x => x.Active == active).ToList();
			foreach (var rootItem in rootItems)
			{
				rootItem.LoadSubItems(allItems);
			}

			return rootItems;
		}

		async Task<IEnumerable<Manual>> FetchAllAsync() => await _manualRepository.ListAsync(new ManualFilterSpecification());

		public async Task<Manual> GetByIdAsync(int id) => await _manualRepository.GetByIdAsync(id);

		public async Task<Manual> CreateAsync(Manual manual) => await _manualRepository.AddAsync(manual);

		public async Task UpdateAsync(Manual manual) => await _manualRepository.UpdateAsync(manual);

		public async Task UpdateAsync(Manual existingEntity, Manual model) => await _manualRepository.UpdateAsync(existingEntity, model);

		public async Task UpdateOrderAsync(int target, int replace, bool up)
		{
			var targetEntity = await _manualRepository.GetByIdAsync(target);
			int targetOrder = targetEntity.Order;

			var replaceEntity = await _manualRepository.GetByIdAsync(replace);
			int replaceOrder = replaceEntity.Order;

			targetEntity.Order = replaceOrder;
			replaceEntity.Order = targetOrder;

			if (targetEntity.Order == replaceEntity.Order)
			{
				if (up) replaceEntity.Order += 1;
				else targetEntity.Order += 1;
			}

			_manualRepository.UpdateRange(new List<Manual> { targetEntity, replaceEntity });
		}

		public async Task RemoveAsync(Manual manual)
		{
			manual.Removed = true;
			await _manualRepository.UpdateAsync(manual);
		}

		public Manual GetById(int id)
		{
			var spec = new ManualFilterSpecification(id);
			return _manualRepository.GetSingleBySpec(spec);

		}

		public Feature GetFeatureById(int id) => _featureRepository.GetSingleBySpec(new FeatureFilterSpecification(id));

		public async Task<Feature> GetFeatureByIdAsync(int id) => await _featureRepository.GetByIdAsync(id);

		public async Task<Feature> CreateFeatureAsync(Feature feature) => await _featureRepository.AddAsync(feature);

		public async Task UpdateAsync(Feature existingEntity, Feature feature) => await _featureRepository.UpdateAsync(existingEntity, feature);
	}
}
