using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Views
{
    public class OrderRequest
    {
        public int TargetId { get; set; }

        public int ReplaceId { get; set; }

        public bool Up { get; set; }
    }
}
