using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.DataAccess;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class PayFilterSpecification : BaseSpecification<Pay>
    {
        public PayFilterSpecification() : base(item => !item.Removed)
        {
           
        }

        public PayFilterSpecification(int id) : base(item => !item.Removed && item.Id == id)
        {
           
        }

        public PayFilterSpecification(string code) : base(item => !item.Removed && item.Code == code)
        {

        }

        public PayFilterSpecification(PayWay payWay) : base(item => !item.Removed && item.PayWayId == payWay.Id)
        {
           
        }

    }
}
