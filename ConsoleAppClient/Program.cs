using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

            CustomDelegatingHandler customDelegatingHandler = new CustomDelegatingHandler();

            HttpClient clienthttp = HttpClientFactory.Create(customDelegatingHandler);


            Console.WriteLine("Enter your username:");

            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");

            string password = Console.ReadLine();

            // Create a new product
            login.Username = username;
            login.Password = password;

            string loginUrl = "https://localhost:44315/api/login";

            HttpResponseMessage response = clienthttp.PostAsJsonAsync(new Uri(loginUrl), login).Result;

            //HttpResponseMessage response = client.GetAsync(new Uri(loginUrl)).Result;

            //string responseBody = response.Content.ReadAsStringAsync().Result;

            if (response.IsSuccessStatusCode)
            {
                string responseString = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseString);
                Console.WriteLine("HTTP Status: {0}, Reason {1}. Press ENTER to exit", response.StatusCode, response.ReasonPhrase);
            }
            else
            {
                Console.WriteLine("Failed to call the API. HTTP Status: {0}, Reason {1}", response.StatusCode, response.ReasonPhrase);
            }

            //string responseBody = response.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(responseBody);
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

        public class CustomDelegatingHandler : DelegatingHandler
        {
            //Obtained from the server earlier, APIKey MUST be stored securly and in App.Config
            private string APPId = "4d53bce04ec34c0a911182d4c228ee6c"; //"4d53bce03ec34c0a911182d4c228ee6c";
            private string APIKey = "A94reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc="; //"A93reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc=";

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {

                HttpResponseMessage response = null;
                string requestContentBase64String = string.Empty;

                string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

                string requestHttpMethod = request.Method.Method;

                //Calculate UNIX time
                DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan timeSpan = DateTime.UtcNow - epochStart;
                string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

                //create random nonce for each request
                string nonce = Guid.NewGuid().ToString("N");

                //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
                if (request.Content != null)
                {
                    byte[] content = await request.Content.ReadAsByteArrayAsync();
                    MD5 md5 = MD5.Create();
                    //Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                    byte[] requestContentHash = md5.ComputeHash(content);
                    requestContentBase64String = Convert.ToBase64String(requestContentHash);
                }

                //Creating the raw signature string
                string signatureRawData = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

                var secretKeyByteArray = Convert.FromBase64String(APIKey);

                byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

                using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
                {
                    byte[] signatureBytes = hmac.ComputeHash(signature);
                    string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                    //Setting the values in the Authorization header using custom scheme (amx)
                    request.Headers.Authorization = new AuthenticationHeaderValue("amx", string.Format("{0}:{1}:{2}:{3}", APPId, requestSignatureBase64String, nonce, requestTimeStamp));
                }

                response = await base.SendAsync(request, cancellationToken);

                return response;
            }
        }

        private void GenerateAPPKey()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32]; //256 bit
                cryptoProvider.GetBytes(secretKeyByteArray);
                var APIKey = Convert.ToBase64String(secretKeyByteArray);
            }
        }

    }
}

