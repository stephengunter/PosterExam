using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Models
{

    public class RequestViewModel
    {
        public string Id = Guid.NewGuid().ToString();

        public string Origin { get; set; }

        public string Content { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}