using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayMvc.Models
{
    public class ReportViewModel
    {
        public string Id = Guid.NewGuid().ToString();

        public int Status { get; set; }

        public string Url { get; set; }

        public string Content { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}