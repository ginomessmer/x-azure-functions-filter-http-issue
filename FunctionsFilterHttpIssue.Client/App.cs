using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace FunctionsFilterHttpIssue.Client
{
    public class App
    {
        public string EndpointUrl { get; set; }

        public async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                WriteLine("Hey there! Please specify the URL endpoint of your Azure Function (e.g. http://localhost:7071):");
                this.EndpointUrl = Console.ReadLine();

                WriteLine("What's your name?");
                string ownName = Console.ReadLine();

                var names = new List<string>()
                {
                    ownName,
                    "John Doe",
                    "Jane Doe",
                    "Lorem Ipsum"
                };

                foreach(var name in names)
                {
                    WriteLine($"Testing name: {name}");
                    HttpResponseMessage response = await client.GetAsync(this.EndpointUrl + $"/api/Default?name={name}");

                    if (!response.IsSuccessStatusCode) ForegroundColor = ConsoleColor.Red;
                    WriteLine($"HTTP {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

                    ResetColor();
                    WriteLine();
                }
            }
        }
    }
}