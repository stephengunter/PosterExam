using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Models
{
    public class PayForm
    {
        public string Id { get; set; }  //支付廠商帳單Id

        public int BillId { get; set; } //我方帳單Id
        //public string BillCode { get; set; }

        public string Money { get; set; }

        public string PayWay { get; set; }
       
    }
}
