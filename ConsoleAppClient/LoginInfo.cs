﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppClient
{
    public class LoginInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Answers> Responses { get; set; }

    }
}
