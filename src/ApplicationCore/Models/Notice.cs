using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Entities;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
    public class Notice : BaseRecord
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public bool Top { get; set; }

        public int Clicks { get; set; }

        public bool Public { get; set; }

        public ICollection<Receiver> Receivers { get; set; } = new List<Receiver>();

    }


    public class Receiver : BaseEntity
    {
        public int NoticeId { get; set; }

        public string UserId { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public Notice Notice { get; set; }

        public User User { get; set; }


        public bool HasReceived => ReceivedAt.HasValue;
    }
}
