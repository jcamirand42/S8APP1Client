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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpResponseMessage response = client.GetAsync(new Uri("https://localhost:44315/api/values")).Result;

            string responseBody = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(responseBody);
        }

    }
}
