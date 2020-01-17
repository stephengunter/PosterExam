﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationCore.Exceptions;

namespace ApplicationCore.Helpers
{
    public static class QuestionHelpers
    {
        public static ExamQuestion ConversionToExamQuestion(this Question question, int optionCount)
        {
            if (optionCount > question.Options.Count)
            {
                throw new OptionToLessException($"OptionToLess. QuestionId: {question.Id} , OptionCount: {question.Options.Count} , Need: {optionCount} ");
            }
            var options = question.Options.ToList().Shuffle(optionCount);
            var examQuestion = new ExamQuestion
            {
                QuestionId = question.Id,
                Options = options,
                OptionIds = String.Join(",", options.Select(x => x.Id))
            };

            return examQuestion;
        }
    }

    //public class ExamQuestion : BaseEntity
    //{
    //    public int ExamPartId { get; set; }
    //    public int Order { get; set; }
    //    public int QuestionId { get; set; }
    //    public int AnswerIndex { get; set; }
    //    public string OptionIds { get; set; } // 0,1,4,5

    //    public string UserAnswerIndex { get; set; }

    //    public ExamPart ExamPart { get; set; }
    //    public Question Question { get; set; }
    //}
}
