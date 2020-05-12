﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Exceptions
{
    public class CreateEcPayTradeFailed : Exception
    {
        public CreateEcPayTradeFailed(string msg) : base(msg)
        {

        }

        public CreateEcPayTradeFailed(string msg, Exception ex) : base(msg, ex)
        {

        }
    }
}