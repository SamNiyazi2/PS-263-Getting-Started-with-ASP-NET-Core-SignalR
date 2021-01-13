using System;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // 01/12/2021 04:13 am - SSN - [20210112-0409] - [002] - M04 - Working with ASP.NET Core SignalR



            // Added code to pickup application URL from launchSettings.json
            string targetApplicationName = JSON_Util.getEnvironmentVariable("profiles.WiredBrain.applicationUrl");

            if (string.IsNullOrEmpty(targetApplicationName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Could not get applicationUrl from WiredBrain profile.");
                Console.WriteLine("Process terminated.");
                Console.ResetColor();
                Console.WriteLine("hit any key.");
                Console.ReadKey();
                return;
            }



            //                .WithUrl("http://localhost:60907/coffeehub")


            Console.WriteLine("");
            Console.WriteLine($"Hub: {targetApplicationName}");
            Console.WriteLine("");


            var connection = new HubConnectionBuilder()
                .WithUrl($"{targetApplicationName}/coffeehub")
                .AddMessagePackProtocol()
                .Build();

            connection.On<Order>("NewOrder", (order) =>
                Console.WriteLine($"OrderNo {order.OrderNo}: Order for {order.Product}. Size: {order.Size}"));

            connection.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Listening. Press a key to quit");
            Console.ReadKey();
        }
    }
}
