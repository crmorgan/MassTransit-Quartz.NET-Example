using System;
using System.Threading.Tasks;
using MassTransit;
using Messages;

namespace QuartzTest.Endpoint
{
    public class StartTestConsumer : IConsumer<StartTest>, IConsumer<TestDelay>
    {
        public async Task Consume(ConsumeContext<StartTest> context)
        {
            Program.NumberOfReceived++;
            Console.WriteLine($"Received StartTest {context.Message.TestId}");
            await context.SchedulePublish(DateTime.Now.AddSeconds(10), new TestDelay{TestId = context.Message.TestId});
        }

        public Task Consume(ConsumeContext<TestDelay> context)
        {
            Program.NumberOfDelayReceived++;
            Console.WriteLine($"Received TestDelay {context.Message.TestId} - ({Program.NumberOfReceived-Program.NumberOfDelayReceived}) *************************************************");
            return Task.CompletedTask;
        }
    }
}