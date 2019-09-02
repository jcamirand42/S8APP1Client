﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleAppClient
{


    class Program
    {

        static HttpClient client = new HttpClient();
        static LoginInfo login = new LoginInfo();

        static List<Survey> objs = new List<Survey>();

        static Survey selectedSurvey = new Survey();

        static List<string> answers = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            userLogin();
            GetSurveys();
        }
        
        static void userLogin()
        {
            Console.WriteLine("Enter your username:");

            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");

            string password = Console.ReadLine();

            // Create a new product
            login.Username = username;
            login.Password = password;

            string loginUrl = "https://localhost:44315/api/login";

            HttpResponseMessage response2 = client.PostAsJsonAsync(new Uri(loginUrl), login).Result;

            HttpResponseMessage response = client.GetAsync(new Uri(loginUrl)).Result;

            string responseBody = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseBody);
        }

        static void GetSurveys()
        {
            HttpResponseMessage response = client.GetAsync(new Uri("https://localhost:44315/api/survey")).Result;

            string responseBody = response.Content.ReadAsStringAsync().Result;

            objs = JsonConvert.DeserializeObject<List<Survey>>(responseBody);

            Console.WriteLine("huehuehue");

            PrintSurveys();

            SelectSurvey();

            AnswerQuestions();

            SendAnswers();

        }

        static void PrintSurveys()
        {
            foreach (Survey s in objs)
            {
                Console.WriteLine(s.id + " " + s.name);
            }
        }

        static void SelectSurvey()
        {
            Console.WriteLine("Enter survey id to start");

            string entree = Console.ReadLine();

            selectedSurvey = objs.Find(p => p.id.ToString() == entree);

        }

        static void AnswerQuestions()
        {
            foreach (Question q in selectedSurvey.questions)
            {
                Console.WriteLine(q.statement);
                foreach (KeyValuePair<string, string> entry in q.choices)
                {
                    Console.WriteLine(entry.Key + ": " + entry.Value);
                }
                GetUserAnswer(q.choices);
            }
        }

        static void GetUserAnswer(Dictionary<string, string> choices)
        {
            string entry = Console.ReadLine();
            entry = entry.ToLower();
            while (!choices.ContainsKey(entry))
            {
                Console.WriteLine("choix invalide");
                entry = Console.ReadLine();
                entry = entry.ToLower();
            }
            answers.Add(entry);
        }

        static void SendAnswers()
        {
            string answersUrl = "https://localhost:44315/api/answers";
            Answers newAnswers = new Answers() { PersonId = 1, SurveyId = selectedSurvey.id, responses = answers };
            HttpResponseMessage response2 = client.PostAsJsonAsync(new Uri(answersUrl), newAnswers).Result;
        }

    }
}

