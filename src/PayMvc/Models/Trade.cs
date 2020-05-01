using ECPay.Payment.Integration.SPCheckOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Models
{
    public class TradeRequestModel
    {
        public string AppName { get; set; }

        public string Code { get; set; }

        public int Amount { get; set; }

        public int ValidDays { get; set; }
    }

    public class EcPayTradeModel
    {
        public TradeSPToken TokenModel { get; set; }

        public string OriginURL { get; set; }
        public string CheckOutURL { get; set; }
        public string PaymentType { get; set; }
    }

    public class TradeResultModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}