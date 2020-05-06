﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
    public class RequestViewModel
    {
        public string Id = Guid.NewGuid().ToString();

        public string Origin { get; set; }

        public string Content { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}
