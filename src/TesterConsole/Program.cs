using System;
using System.Threading.Tasks;
using Messages;
using ServiceBus;

namespace TesterConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Super Duper MassTransit Tester";

            var bus = BusConfigurator.CreateBus();
            var busUri = BusConfigurator.GetBusUri("quartz-test");
            var endpoint = await bus.GetSendEndpoint(busUri);

            OutputPrompt();

            var testRunId = Guid.NewGuid();

            do
            {
                var input = Console.ReadLine();

                if (input == "q")
                    break;

                if (input.StartsWith("t"))
                {
                    var testRuns = int.Parse(input.Split(':')[1]);

                    for (var i = 0; i < testRuns; i++)
                    {
                        var testId = $"{testRunId}-{i + 1}";
                        await endpoint.Send(new StartTest { TestId = testId });
                        Console.WriteLine($"Sent StartTest {testId}.");
                    }

                    Console.WriteLine();
                    OutputPrompt();
                }

            } while (true);
        }

        private static void OutputPrompt()
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Enter 't:n' to run one or n number of tests or 'q' to quit");
            Console.ForegroundColor = currentColor;
        }
    }
}
