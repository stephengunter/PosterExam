using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Models
{
    public class SubscribesIndexViewModel
    {
        public List<SubscribeViewModel> Records { get; set; } = new List<SubscribeViewModel>();

        public SubscribeViewModel Current { get; set; }

        public PlanViewModel Plan { get; set; }

        public List<PayWayViewModel> PayWays { get; set; } = new List<PayWayViewModel>();
    }

    public class SubscribeEditForm
    {
        public PlanViewModel Plan { get; set; }

        public int PayWay { get; set; }
    }
}
