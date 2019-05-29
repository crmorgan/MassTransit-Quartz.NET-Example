# MassTransit-Quartz.NET-Example
This is a simple example of using MassTransit and Quartz.NET to schedule message delivery.

## Startup Projects
Setup the solution to start the both the **TesterConsole** and **QuartzTest.Endpoint** console application projects.

## RabbitMQ and SQL Server
There is a **docker-compose.yaml** file that will start the required servers.  Start them with the command:

```
docker-compose up -d
```

## Database Requirements
This example is using SQL Server to store Quartz.NET data.

1. If you are using the SQL Server container connect to it in SSMS with the server name **127.0.0.1,1433** and password of **Password1**.
2. Execute the **QuartzDbSetup.sql** script to create a QuartzExample database and the necessary tables.

## Sending Test Messages
Run the solution which will start the two console applications.  In the **TesterConsole** console enter `t:[NumberOfRuns]` and press enter to publish *n* number of **StartTest** messages to RabbitMQ.  For example if you want two tests run enter `t:2`.

The **StartTestConsumer** consumer will handles the **StartTest** commands which then schedules a **TestDelay** event to be published in ten seconds.  The **TestDelay** event is also handled by **StartTestConsumer** which then ends the test.

## Fix Failed Jobs 

If a job fails during creation it will be left in a error status in the database and will not be rescheduled.  Run the following update statement to put them back into a waiting status so they will get run again.

Script to put failed job into waiting state:
```sql
update QRTZ_TRIGGERS set TRIGGER_STATE = 'WAITING', NEXT_FIRE_TIME = 636937197657316850 where TRIGGER_STATE = 'ERROR'
```

The **NEXT_FIRE_TIME** column a UTC date in ticks the specifies when the job will be run again.

You can get the next fire time ticks value using C#:
```cs
Console.WriteLine(DateTime.UtcNow.Ticks);
```
