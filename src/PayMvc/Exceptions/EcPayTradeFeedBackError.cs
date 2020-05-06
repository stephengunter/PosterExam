using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Exceptions
{
    public class EcPayTradeFeedBackError : Exception
    {
        public EcPayTradeFeedBackError(string msg, Exception ex) : base(msg, ex)
        {

        }
    }
}