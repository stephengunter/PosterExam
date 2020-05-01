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

namespace PayMvc.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Route("api/ecpay/{action}")]
    public class EcPayController : ApiController
    {
        private readonly IAppLogger _appLogger;

        private readonly string _AppUrl;

        private readonly string _ECPayUrl;
        private readonly string _ECPayHashKey;
        private readonly string _ECPayHashIV;
        private readonly string _ECPayMerchantID;

        public EcPayController()
        {
            var settings = ConfigurationManager.AppSettings;
            _AppUrl = settings["AppUrl"];

            _ECPayUrl = settings["ECPayUrl"];
            _ECPayHashKey = settings["ECPayHashKey"];
            _ECPayHashIV = settings["ECPayHashIV"];
            _ECPayMerchantID = settings["ECPayMerchantID"];

            var folderPath = settings["DataPath"];
            _appLogger = new AppLogger(folderPath, NLog.LogManager.GetCurrentClassLogger());
        }

        string CreateTradeURL => $"{_ECPayUrl}/SP/CreateTrade";
        string CheckOutURL => $"{_ECPayUrl}/SP/SPCheckOut";

        string PayStoreUrl => $"{_AppUrl}/api/ecpay/store";

        [HttpPost]
        public EcPayTradeModel Create([FromBody] TradeRequestModel model)
        {
            TradeSPToken resultModel = null;
            try
            {
                using (SPCheckOutApi oPayment = new SPCheckOutApi())
                {
                    oPayment.ServiceURL = CreateTradeURL;
                    oPayment.HashKey = _ECPayHashKey;
                    oPayment.HashIV = _ECPayHashIV;
                    oPayment.Send.MerchantID = _ECPayMerchantID;

                    oPayment.Send.MerchantTradeNo = model.Code;
                    oPayment.Send.ItemName = "訂閱會員";   //商品名稱
                    oPayment.Send.ReturnURL = PayStoreUrl;  //付款完成通知回傳網址
                    oPayment.Send.TotalAmount = Convert.ToUInt32(model.Amount);  //交易金額
                    oPayment.ATM.ExpireDate = model.ValidDays;  //允許繳費有效天數
                    oPayment.Send.TradeDesc = model.AppName;  //交易描述

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
                        _appLogger.LogException(new CreateEcPayTradeFailed(result));
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
                        OriginURL = _ECPayUrl
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
        public string Store()
        {
            string title = PayStoreUrl;
            var jsonContent = Request.Content.ReadAsStringAsync().Result;
            _appLogger.LogInfo($"{PayStoreUrl} , got request: {jsonContent}");

            //List<string> enErrors = new List<string>();
            //Hashtable htFeedback = null;
              
            //using (AllInOne oPayment = new AllInOne())
            //{
            //    oPayment.HashKey = _ECPayHashKey;
            //    oPayment.HashIV = _ECPayHashIV;
            //    /* 取回付款結果 */
            //    enErrors.AddRange(oPayment.CheckOutFeedback(ref htFeedback));
            //}
          
          
            return "Damn";
        }
    }
}
