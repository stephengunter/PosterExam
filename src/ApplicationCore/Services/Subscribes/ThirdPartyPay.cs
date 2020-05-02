using ApplicationCore.Settings;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Text;
using Newtonsoft.Json;
using ApplicationCore.Helpers;
using ApplicationCore.Exceptions;
using ApplicationCore.Views;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using ApplicationCore.Logging;

namespace ApplicationCore.Services
{
    public interface IThirdPartyPayService
    {
        Task<EcPayTradeModel> CreateEcPayTradeAsync(Pay pay, int amount);

    }

    public class EcPayService : IThirdPartyPayService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppSettings _appSettings;
		private readonly SubscribesSettings _subscribesSettings;

        public EcPayService(IHttpClientFactory clientFactory,IOptions<AppSettings> appSettings,
            IOptions<SubscribesSettings> subscribesSettings)
        {
			_appSettings = appSettings.Value;
			_subscribesSettings = subscribesSettings.Value;
            _clientFactory = clientFactory;

            Client = _clientFactory.CreateClient(Consts.TradeRemoteApiName);
        }

        string PayUrl => _appSettings.PayUrl;

        public HttpClient Client { get; }

        public async Task<EcPayTradeModel> CreateEcPayTradeAsync(Pay pay, int amount)
        {
            var model = new TradeRequestModel
            {
                AppName = _appSettings.Name,
                Code = pay.Code,
                Amount = amount,
                ValidDays = _subscribesSettings.BillDaysToExpire
            };

            string action = "api/ecpay/create";

            try
            {
                var response = await Client.PostAsync(action, new JsonContent(model));
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var ecPayTradeModel = JsonConvert.DeserializeObject<EcPayTradeModel>(result);

                    if (ecPayTradeModel.HasToken)
                    {
                        //only here success
                        ecPayTradeModel.PaymentType = pay.PayWay;
                        return ecPayTradeModel;
                    }
                    else throw new CreateThirdPartyTradeFailed(pay);
                }
                else
                {
                    throw new RemoteApiException((int)response.StatusCode, $"{PayUrl}/{action}");
                }
            }
            catch (Exception ex)
            {
                throw new RemoteApiException($"{PayUrl}/{action}", ex);
            }

        }

    }
}
