using ECPay.Payment.Integration.SPCheckOut;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayMvc.Models;
using Newtonsoft.Json;
using PayMvc.Helpers;
using PayMvc.Logging;
using PayMvc.Exceptions;
using System.Web.Http.Cors;
using System.Collections;
using ECPay.Payment.Integration;
using PayMvc.Config;
using System.Threading.Tasks;

namespace PayMvc.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/ecpay/{action}")]
    public class EcPayController : ApiController
    {
        private readonly IAppLogger _appLogger;

        private readonly string _AppUrl;

        private readonly ThirdPartySettings _ecPaySettings;
        private readonly ClientSettings _posterClientSettings;

        const string ATM_PAYWAY = "ATM";
        const string CREDIT_PAYWAY = "CREDIT";

        public EcPayController()
        {
            var settings = ConfigurationManager.AppSettings;
            _AppUrl = settings["AppUrl"];

            var folderPath = settings["DataPath"];
            _appLogger = new AppLogger(folderPath, NLog.LogManager.GetCurrentClassLogger());

            var thirdPartiesCection = ConfigurationManager.GetSection("thirdParties");
            if (thirdPartiesCection != null)
            {
                var providers = (thirdPartiesCection as ThirdPartySection).Providers;
                _ecPaySettings = providers[0];
            }

            var clientsSection = ConfigurationManager.GetSection("clientsSection");
            if (clientsSection != null)
            {
                var clients = (clientsSection as ClientSection).Clients;
                _posterClientSettings = clients[0];
            }
        }

        string ECPayUrl => _ecPaySettings.Url;
        string ECPayHashKey => _ecPaySettings.HashKey;
        string ECPayHashIV => _ecPaySettings.HashIV;
        string ECPayMerchantID => _ecPaySettings.MerchantID;

        string CreateTradeURL => $"{ECPayUrl}/SP/CreateTrade";
        string CheckOutURL => $"{ECPayUrl}/SP/SPCheckOut";
        string PayStoreUrl => $"{_AppUrl}/api/ecpay/store";

        string PosterClientUrl => _posterClientSettings.Url;

        string GetPaymentType(string type)
        {
            if (type.StartsWith(ATM_PAYWAY)) return ATM_PAYWAY;
            else if(type.StartsWith(CREDIT_PAYWAY)) return CREDIT_PAYWAY;
            else return "";
        }

        [HttpGet]
        public async Task<string> Test()
        {
            var tradeResultModel = new TradeResultModel()
            {
                Provider = _ecPaySettings.Id,
                Code = "20200503E4KTE45HXZ2W",
                TradeNo = "2005022005374216",
                Amount = 190,
                Payed = true,
                PayedDate = "2020/05/02 20:50:18"
            };

            await NotifyClientAsync(tradeResultModel);

            return "test";
        }

           

        [HttpPost]
        public EcPayTradeModel Create([FromBody] TradeRequestModel model)
        {
            TradeSPToken resultModel = null;
            try
            {
                using (SPCheckOutApi oPayment = new SPCheckOutApi())
                {
                    oPayment.ServiceURL = CreateTradeURL;
                    oPayment.HashKey = ECPayHashKey;
                    oPayment.HashIV = ECPayHashIV;
                    oPayment.Send.MerchantID = ECPayMerchantID;

                    oPayment.Send.MerchantTradeNo = model.Code;
                    oPayment.Send.ItemName = "訂閱會員";   //商品名稱
                    oPayment.Send.ReturnURL = PayStoreUrl;  //付款完成通知回傳網址
                    oPayment.Send.TotalAmount = Convert.ToUInt32(model.Amount);  //交易金額
                    oPayment.ATM.ExpireDate = model.ValidDays;  //允許繳費有效天數
                    oPayment.Send.TradeDesc = model.AppId;  //交易描述

                    oPayment.Send.NeedExtraPaidInfo = "N";  //額外回傳參數
                    oPayment.Send.ClientBackURL = ""; //Client端返回特店的按鈕


                    oPayment.ATM.PaymentInfoURL = PayStoreUrl;

                    string result = oPayment.Excute();
                    try
                    {
                        resultModel = JsonConvert.DeserializeObject<TradeSPToken>(result);
                    }
                    catch (Exception ex)
                    {
                        _appLogger.LogException(new CreateEcPayTradeFailed(result, ex));
                        return new EcPayTradeModel();
                    }

                }

                if (resultModel.RtnCode.ToInt() == 1)
                {
                    //success
                    return new EcPayTradeModel
                    {
                        TokenModel = resultModel,
                        CheckOutURL = CheckOutURL,
                        OriginURL = ECPayUrl,
                    };
                }
                else
                {
                    //failed
                    _appLogger.LogException(new CreateEcPayTradeFailed(JsonConvert.SerializeObject(resultModel)));

                    return new EcPayTradeModel();
                }

            }
            catch (Exception ex)
            {
                _appLogger.LogException(ex);
                return new EcPayTradeModel();
            }

        }

        [HttpPost]
        public async Task<string> Store()
        {
            await _appLogger.LogRequestAsync(Request);

            List<string> enErrors = new List<string>();
            Hashtable htFeedback = null;

            //bool notifyClientSuccess = false;

            try
            {
                using (AllInOne oPayment = new AllInOne())
                {
                    oPayment.HashKey = ECPayHashKey;
                    oPayment.HashIV = ECPayHashIV;
                    /* 取回付款結果 */
                    enErrors.AddRange(oPayment.CheckOutFeedback(ref htFeedback));
                }

                if (enErrors.IsNullOrEmpty())
                {
                    _appLogger.LogInfo($"htFeedback: {JsonConvert.SerializeObject(htFeedback)}");

                    var tradeResultModel = new TradeResultModel()
                    {
                        Provider = _ecPaySettings.Id,
                        Code = htFeedback["MerchantTradeNo"].ToString(),
                        TradeNo = htFeedback["TradeNo"].ToString(),
                        Amount = htFeedback["TradeAmt"].ToString().ToInt()
                    };

                    var rtnCode = htFeedback["RtnCode"].ToString().ToInt();
                    if (rtnCode == 1) //付款成功
                    {
                        bool simulatePaid = false;
                        if (htFeedback.ContainsKey("SimulatePaid")) simulatePaid = htFeedback["SimulatePaid"].ToString().ToInt() > 0;

                        if (!simulatePaid)
                        {
                            //真的付款紀錄
                            tradeResultModel.Payed = true;
                            tradeResultModel.PayedDate = htFeedback["PaymentDate"].ToString();
                            tradeResultModel.PayWay = GetPaymentType(htFeedback["PaymentType"].ToString());
                        }
                    }
                    else if (rtnCode == 2) //ATM 取號成功
                    {
                        tradeResultModel.Payed = false;
                        tradeResultModel.PayWay = GetPaymentType(htFeedback["PaymentType"].ToString());
                        tradeResultModel.BankCode = htFeedback["BankCode"].ToString();
                        tradeResultModel.BankAccount = htFeedback["vAccount"].ToString();
                        tradeResultModel.ExpireDate = htFeedback["ExpireDate"].ToString();
                    }
                    else
                    {
                        //Failed
                    }

                    tradeResultModel.Data = JsonConvert.SerializeObject(htFeedback);

                    //notifyClientSuccess = await NotifyClientAsync(tradeResultModel);

                }
            }
            catch (Exception ex)
            {
                _appLogger.LogException(new EcPayTradeFeedBackError(ex.Message, ex));

                enErrors.Add(ex.Message);

            }


            if (enErrors.IsNullOrEmpty())
            {
                string okMsg = "1|OK";
                _appLogger.LogInfo($"handle trade result success. return value = {okMsg}");

                return okMsg;

                //if (notifyClientSuccess) return "1|OK";
                //else return "0|custom error";
            }
            else
            {
                string failedMsg = String.Format("0|{0}", String.Join("\\r\\n", enErrors));
                _appLogger.LogInfo($"trade result failed. return value = {failedMsg}");

                return failedMsg;
            }
        }


        async Task<bool> NotifyClientAsync(TradeResultModel model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new JsonContent(model);
                    var response = await httpClient.PostAsync(PosterClientUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        _appLogger.LogReport(PosterClientUrl, JsonConvert.SerializeObject(model), 200);
                        return true;

                    }
                    else
                    {
                        _appLogger.LogReport(PosterClientUrl, JsonConvert.SerializeObject(model), (int)response.StatusCode);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                _appLogger.LogException(new RemoteApiException(PosterClientUrl, ex));
                return false;
            }
        }

    }
}
