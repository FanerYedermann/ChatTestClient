using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChatTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Hosting environment: {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")}");
            Run().Wait();
        }

        private static async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5001/chathub")
                .WithAutomaticReconnect()
                .Build();

            connection.On("ReceiveMessage", (string user, string message) =>
            {
                Console.WriteLine( $"{user}: {message}");
            });
            await connection.StartAsync();

            if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development")
            {
                // await RunInteractive(connection);
                await RunAutomatic(connection);
            }
            else
            {
                await RunAutomatic(connection);
            }

        }

        private static async Task RunAutomatic(HubConnection connection)
        {
            var username = Guid.NewGuid().ToString();
            for (var i = 0; i < 1000000; i++)
            {
                await connection.SendAsync("SendMessage", username, $"Message {i}");
                await Task.Delay(100);
            }
        }

        private static async Task RunInteractive( HubConnection connection )
        {
            Console.WriteLine("Who are you?");
            var username = Console.ReadLine();
            Console.WriteLine("Please write a message: ");
            var input = string.Empty;
            while ((input = Console.ReadLine()) != "q" )
            {
                await connection.SendAsync("SendMessage", username, input);
            }
        }
    }
}