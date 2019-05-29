using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Quartz;
using Quartz.Impl;

namespace ServiceBus
{
    public static class BusConfigurator
    {

        public static IBusControl CreateBus(Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost> registrationAction = null)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
                                                   {
                                                       var host = cfg.Host("localhost", "/", h =>
                                                                                         {
                                                                                             h.Username("guest");
                                                                                             h.Password("guest");
                                                                                         });

                                                       registrationAction?.Invoke(cfg, host);
                                                   });
        }

        public static Uri GetBusUri(string queue)
        {
            return new Uri($"rabbitmq://localhost/{queue}");
        }

        public static async Task<IScheduler> CreateScheduler()
        {
            NameValueCollection properties = new NameValueCollection();

            properties["quartz.scheduler.instanceName"] = "TestQuartzScheduler";
            properties["quartz.scheduler.instanceId"] = "instance_one";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "19";
            properties["quartz.serializer.type"] = "binary";
            properties["quartz.jobStore.misfireThreshold"] = "60000";
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz";
            properties["quartz.jobStore.useProperties"] = "false";
            properties["quartz.jobStore.dataSource"] = "default";
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            properties["quartz.jobStore.clustered"] = "true";
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";

            properties["quartz.dataSource.default.connectionString"] = "Server=.,1433; Database=QuartzExample; User Id=sa; Password=Password1;";
            properties["quartz.dataSource.default.provider"] = "SqlServer";

            var schedulerFactory = new StdSchedulerFactory(properties);
            return await schedulerFactory.GetScheduler();
        }
    }
}