using Newtonsoft.Json;
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

        static LoginInfo tempLog = new LoginInfo();
        static LoginInfo login = new LoginInfo();

        static List<Survey> objs = new List<Survey>();

        static Survey selectedSurvey = new Survey();

        static List<string> answers = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            getAPIAuthorization();
            userLogin();
            GetSurveys();
            PrintSurveys();
            SelectSurvey();
            AnswerQuestions();
            SendAnswers();
        }
        
        static void getAPIAuthorization()
        {
            
            //Verification de la clef d'autorisation
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey");

            // Test de l'autorisation
            /*string baseUrl = "https://localhost:44315/api/login";
            HttpResponseMessage response = client.GetAsync(new Uri(baseUrl)).Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                APIKeyNotValid(response);
            }*/
        }

        static void userLogin()
        {
            Console.WriteLine("Nom d'utilisateur: ");

            string username = Console.ReadLine();

            Console.WriteLine("Mot de passe: ");

            string password = Console.ReadLine();

            LoginInfo log = new LoginInfo();

            // Create a new product
            tempLog.Username = username;
            tempLog.Password = password;

            string loginUrl = "https://localhost:44315/api/login";

            HttpResponseMessage response = client.PostAsJsonAsync(new Uri(loginUrl), tempLog).Result;
            string test = response.StatusCode.ToString();

            if (response.IsSuccessStatusCode)
            {
                string responseString = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
                login = JsonConvert.DeserializeObject<LoginInfo>(responseString);
                //login = tempLog;
                
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                Console.WriteLine("Votre nom d'utilisateur ou votre mot de passe n'est pas valide");
                userLogin();
            }
            else
            {
                APIKeyNotValid(response);
            }

        }

        static void GetSurveys()
        {
            HttpResponseMessage response = client.GetAsync(new Uri("https://localhost:44315/api/survey")).Result;

            if (response.IsSuccessStatusCode)
            { 
                string responseBody = response.Content.ReadAsStringAsync().Result;
                objs = JsonConvert.DeserializeObject<List<Survey>>(responseBody);

            }
            else
            {
                APIKeyNotValid(response);
            }
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
            Console.WriteLine("Veuiller entrer l'identifiant de votre sondage (1 ou 2)");

            string entree = Console.ReadLine();

            int number;
            Int32.TryParse(entree, out number);

            if (login.Responses == null)
            {
                login.Responses = new List<Answers>();
            }

            if (login.Responses.FindIndex(p => p.SurveyId == number) < 0)
            { 
                selectedSurvey = objs.Find(p => p.id.ToString() == entree);
                if (selectedSurvey == null)
                {
                    Console.WriteLine("Ce sondage est inexistant");
                    SelectSurvey();
                }
            }
            else
            {
                Console.WriteLine("Vous avez déjà compléter ce sondage");
                SelectSurvey();
            }

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
                Console.WriteLine("Choix invalide");
                entry = Console.ReadLine();
                entry = entry.ToLower();
            }
            answers.Add(entry);
        }

        static void SendAnswers()
        {
            string answersUrl = "https://localhost:44315/api/login";
            Answers newAnswers = new Answers() { SurveyId = selectedSurvey.id, responses = answers };
            login.Responses.Add(newAnswers);
            HttpResponseMessage response = client.PutAsJsonAsync(new Uri(answersUrl), login).Result;
            if (!response.IsSuccessStatusCode)
            {
                APIKeyNotValid(response);
            }
        }

        static void APIKeyNotValid(HttpResponseMessage response)
        {           
            Console.WriteLine("Échec de la connexion avec l'API... HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
            Environment.Exit(0);            
        }

    }
}

