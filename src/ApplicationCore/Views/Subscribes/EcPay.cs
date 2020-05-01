using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
    public class TradeRequestModel
    {
        public string AppName { get; set; }

        public string Code { get; set; }

        public int Amount { get; set; }

        public int ValidDays { get; set; }
    }

    public class EcPayTradeSPToken
    {
        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }
        public string SPToken { get; set; }
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string CheckMacValue { get; set; }
    }

    public class EcPayTradeModel
    {
        public EcPayTradeSPToken TokenModel { get; set; }

        public string OriginURL { get; set; }
        public string CheckOutURL { get; set; }
        public string PaymentType { get; set; }


        public bool HasToken => TokenModel != null && !String.IsNullOrEmpty(TokenModel.SPToken);
    }

    public class EcPayTradeResultModel
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string RtnCode { get; set; }
        public string RtnMsg { get; set; }

        public string TradeNo { get; set; }
        public int TradeAmt { get; set; }


    }
}
