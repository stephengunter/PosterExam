using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System.Linq;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models
{
    public class Resolve : BaseReviewable
    {
        public int QuestionId { get; set; }

        public Question Question { get; set; }

        public string Text { get; set; }

        public string Highlight { get; set; } //json string

        public string Source { get; set; } //json string

        
    }
}
