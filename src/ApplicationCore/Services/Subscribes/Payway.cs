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
	public interface IPayWaysService
	{
		Task<IEnumerable<PayWay>> FetchAsync(bool active = true);

		Task<IEnumerable<PayWay>> FetchAllAsync();
		PayWay GetById(int id);
		Task<PayWay> CreateAsync(PayWay payWay);
	}

	public class PayWaysService : IPayWaysService
	{
		private readonly IDefaultRepository<PayWay> _payWayRepository;

		public PayWaysService(IDefaultRepository<PayWay> payWayRepository)
		{
			this._payWayRepository = payWayRepository;
		}

		public async Task<PayWay> CreateAsync(PayWay payWay) => await _payWayRepository.AddAsync(payWay);

		public async Task<IEnumerable<PayWay>> FetchAsync(bool active = true)
		{
			var payWays = await FetchAllAsync();
			if (payWays.IsNullOrEmpty()) return null;

			return payWays.Where(x => x.Active == active);
		}


		public async Task<IEnumerable<PayWay>> FetchAllAsync() => await _payWayRepository.ListAsync(new PayWayFilterSpecification());

		public PayWay GetById(int id)
		{
			var spec = new PayWayFilterSpecification(id);
			return _payWayRepository.GetSingleBySpec(spec);
		}
	}
}
