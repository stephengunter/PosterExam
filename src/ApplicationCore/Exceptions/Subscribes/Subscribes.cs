using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;

namespace ApplicationCore.Exceptions
{

    public class BillPayedButNoCurrentSubscribe : Exception
    {
        //有已繳帳單, 但是沒有CurrentSubscribe
        public BillPayedButNoCurrentSubscribe(User user, Plan plan) : base($"userId: {user.Id}  , planId: {plan.Id}")
        {

        }
    }
}
