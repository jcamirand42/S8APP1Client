using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppClient
{
    public class Answers
    {
        public int Id { get; set; }
        public string PersonId { get; set; }
        public int SurveyId { get; set; }
        public List<string> responses { get; set; }
    }
}
