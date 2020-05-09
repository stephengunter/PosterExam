using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
    public class Message : BaseRecord
    {
        public string Subject { get; set; }

        public string Content { get; set; }

        public string Email { get; set; }

        public string ReturnContent { get; set; } //json string  => BaseMessageViewModel

        public bool Returned { get; set; }
    }
}
