using ApplicationCore.Helpers;
using Infrastructure.Views;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ApplicationCore.Views
{
    public class PayWayViewModel : BaseRecordView
    {
		public int Id { get; set; }

		public string Code { get; set; }

		public string Title { get; set; }
	}
}
