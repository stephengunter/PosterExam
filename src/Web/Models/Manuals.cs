using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;


namespace Web.Models
{
    public class ManualEditForm
    {
        public ICollection<BaseOption<int>> Parents { get; set; } = new List<BaseOption<int>>();

        public ManualViewModel Manual { get; set; }
    }
}
