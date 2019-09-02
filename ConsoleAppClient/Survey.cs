using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppClient
{
    public class Survey
    {
        public int id { get; set; }
        public string name { get; set; }
        public Question[] questions { get; set; }
    }
}
