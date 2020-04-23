﻿using ApplicationCore.DataAccess;
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
	public interface IPaysService
	{
		Task<Pay> CreateAsync(Pay pay);
		Pay FindByCode(string code);

		Task<IEnumerable<PayWay>> FetchPayWaysAsync(bool active = true);
		Task<IEnumerable<PayWay>> FetchAllPayWaysAsync();
		PayWay FindPayWayByCode(string code);
		PayWay GetPayWayById(int id);
		Task<PayWay> CreatePayWayAsync(PayWay payWay);
	}

	public class PaysService : IPaysService
	{
		private readonly IDefaultRepository<Pay> _payRepository;
		private readonly IDefaultRepository<PayWay> _payWayRepository;

		public PaysService(IDefaultRepository<Pay> payRepository, IDefaultRepository<PayWay> payWayRepository)
		{
			this._payRepository = payRepository;
			this._payWayRepository = payWayRepository;
		}

		public Pay FindByCode(string code) => _payRepository.GetSingleBySpec(new PayFilterSpecification(code));

		public async Task<Pay> CreateAsync(Pay pay) => await _payRepository.AddAsync(pay);



		public async Task<PayWay> CreatePayWayAsync(PayWay payWay) => await _payWayRepository.AddAsync(payWay);

		public async Task<IEnumerable<PayWay>> FetchPayWaysAsync(bool active = true)
		{
			var payWays = await FetchAllPayWaysAsync();
			if (payWays.IsNullOrEmpty()) return null;

			return payWays.Where(x => x.Active == active);
		}


		public async Task<IEnumerable<PayWay>> FetchAllPayWaysAsync() => await _payWayRepository.ListAsync(new PayWayFilterSpecification());

		public PayWay FindPayWayByCode(string code) => _payWayRepository.GetSingleBySpec(new PayWayFilterSpecification(code));

		public PayWay GetPayWayById(int id) => _payWayRepository.GetSingleBySpec(new PayWayFilterSpecification(id));
		
	}
}
