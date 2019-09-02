using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleAppClient
{
    public class LoginInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

    }

    class Program
    {

        static HttpClient client = new HttpClient();

        static async Task<Uri> CreateLogin(LoginInfo product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/login", product);
            //response.EnsureSuccessStatusCode();
            //response = client.PostAsJsonAsync(new Uri(responseUrl), userReponseQuestion1).Result;
            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<LoginInfo> GetLoginAsync(string path)
        {
            LoginInfo product = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<LoginInfo>();
            }
            return product;
        }

        static void ShowLoginInfo(LoginInfo product)
        {
            Console.WriteLine($"Name: {product.Username}\tPrice: " +
                $"{product.Password}");
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string baseUrl = "https://localhost:44315/";

            Console.WriteLine("Enter your username:");

            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");

            string password = Console.ReadLine();

            // Create a new product
            LoginInfo product = new LoginInfo
            {
                Username = username,
                Password = password
            };

            //var url = CreateLogin(product);


            // Get the product
            //product = GetLoginAsync(url.PathAndQuery);
            //ShowLoginInfo(product);
            string loginUrl = "https://localhost:44315/api/login";

            HttpResponseMessage response2 = client.PostAsJsonAsync(new Uri(loginUrl), product).Result;

            HttpResponseMessage response = client.GetAsync(new Uri(loginUrl)).Result;

            string responseBody = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseBody);
        }
    }
}

