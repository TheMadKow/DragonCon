using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Modeling.Models.Common
{
    public enum AnswerType
    {
        Success,
        Error,
        Info
    }

    public class Answer
    {
        public Answer(AnswerType type)
        {
            AnswerType = type;
        }

        public static Answer Success => new Answer(AnswerType.Success);
        public static Answer Error => new Answer(AnswerType.Error);


        public AnswerType AnswerType {get; set;}
        public string Message { get; set; }
        public Exception InternalException { get; set; }
    }
}
