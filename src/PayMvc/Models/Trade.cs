using ECPay.Payment.Integration.SPCheckOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Models
{
    public class TradeRequestModel
    {
        public string AppId { get; set; }

        public string PayWay { get; set; }

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
        public string Provider { get; set; }

        public string PayWay { get; set; }

        public string Code { get; set; }

        public string TradeNo { get; set; }

        public int Amount { get; set; }

        public string BankCode { get; set; }

        public string BankAccount { get; set; }

        public string ExpireDate { get; set; }

        public bool Payed { get; set; }

        public string PayedDate { get; set; }

        public string Data { get; set; } //json string
    }
}