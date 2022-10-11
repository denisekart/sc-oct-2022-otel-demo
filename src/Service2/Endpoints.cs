using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Diagnostics.Metrics;

public class Endpoints
{
    public static async Task PutMessage([FromQuery] string message, Meter meter, ILogger<Endpoints> logger, MessagesContext database)
    {
        var counterName = $"count_{message}";
        logger.LogWarning("Increased observability platform cost detected due to custom metric {metricName} reported", counterName);

        using (var scope = logger.BeginScope(new
        {
            scope = "Creating a counter",
            time = DateTime.Now
        }))
        {
            var counter = meter.CreateCounter<int>(counterName, unit: "times", description: "A dangerous counter");
            logger.LogInformation("The counter will now be incremented by {number}", 1);
            counter.Add(1);
        }

        database.Messages.Add(new Message
        {
            Value = message
        });
        await database.SaveChangesAsync();
    }

    public static async Task<Message> GetMessage(int index, MessagesContext database)
    {
        var startIndex = 0;
        if (index < 0)
        {
            startIndex = await database.Messages.MaxAsync(x => x.Id);
        }
        var indexToFetch = startIndex + index + 1;
        // will throw if index does not exist
        var message = await database.Messages.FirstAsync(x => x.Id == indexToFetch);

        return message;
    }

    public static IResult Ready()
    {
        return Results.Text("Service 2 ready!");
    }
}