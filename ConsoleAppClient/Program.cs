using System;
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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            userLogin();
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
    }
}

