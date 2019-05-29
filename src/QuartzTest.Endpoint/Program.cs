using MassTransit;
using MassTransit.QuartzIntegration;
using ServiceBus;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;

namespace QuartzTest.Endpoint
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IConfiguration Configuration { get; set; }
        public static int NumberOfReceived = 0;
        public static int NumberOfDelayReceived = 0;

        static async Task Main(string[] args)
        {
            Console.Title = "Quartz Test Endpoint";

            Configuration = new ConfigurationBuilder()
                         .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                         .AddJsonFile("appsettings.json", true, true)
                         .Build();

            

            var hostBuilder = new HostBuilder().ConfigureServices(ConfigureServices);

            Logger.Info("Configuring bus and scheduler");

            var scheduler = await BusConfigurator.CreateScheduler();
            var scheduledMessageConsumer = new ScheduleMessageConsumer(scheduler);

            var bus = BusConfigurator
                .CreateBus((cfg, host) =>
                           {
                               cfg.ReceiveEndpoint(host, "quartz-test",
                                                   e =>
                                                   {
                                                       e.PrefetchCount = 16;
                                                       e.Consumer<StartTestConsumer>();
                                                   });
                               cfg.ReceiveEndpoint(host, "delayed",
                                                   e =>
                                                   {
                                                       e.PrefetchCount = 16;
                                                       cfg.UseMessageScheduler(e.InputAddress);
                                                       e.Consumer(() => scheduledMessageConsumer);
                                                   });
                           });


            Console.WriteLine("Staring bus and scheduler");
            await bus.StartAsync();
            scheduler.JobFactory = new MassTransitJobFactory(bus);
            await scheduler.Start();

            Console.WriteLine("Listening for messages");
            Console.WriteLine("Enter 'q' to quit");

            await hostBuilder.RunConsoleAsync();

            Console.WriteLine("Shutting down bus and scheduler");
            await scheduler.Standby();
            await bus.StopAsync();
            await scheduler.Shutdown();
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddLogging(builder =>
                                {
                                    builder.AddNLog(new NLogProviderOptions
                                    {
                                        CaptureMessageTemplates = true,
                                        CaptureMessageProperties = true,
                                    });
                                })
                    .BuildServiceProvider();
        }
    }
}
