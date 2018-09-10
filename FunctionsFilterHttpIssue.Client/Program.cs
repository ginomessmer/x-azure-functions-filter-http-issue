using System;
using System.Threading;

namespace FunctionsFilterHttpIssue.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            new App().RunAsync().Wait();
            Thread.Sleep(-1);
        }
    }
}
