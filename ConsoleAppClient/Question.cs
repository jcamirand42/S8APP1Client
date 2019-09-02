using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppClient
{
    public class Question
    {
        public int id { get; set; }
        public int surveyId { get; set; }
        public string statement { get; set; }
        public Dictionary<string, string> choices { get; set; }
    }
}
