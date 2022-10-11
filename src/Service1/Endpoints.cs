using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shared;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public class Endpoints
{
    public static async Task PutMessage([FromQuery] string message, IHttpClientFactory clientFactory, ILogger<Endpoints> logger)
    {
        using var client = clientFactory.CreateClient("service2");
        await client.PutAsync($"messages?message={message}", JsonContent.Create(new { }));

        logger.LogInformation("Posted message {message} to {service}", message, "/service2");
    }

    public static Task<Message?> GetMessage(int index, IHttpClientFactory clientFactory, ActivitySource activitySource, IMemoryCache cache, Meter meter)
    {
        var counter = meter.CreateCounter<double>("dummy_counter");
        counter.Add(new Random().NextDouble());

        return cache.GetOrCreateAsync(index, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);

            using var activity = activitySource.StartActivity("cache missed");

            activity?.AddEvent(new("start cache invalidation"));

            using var client = clientFactory.CreateClient("service2");
            var message = await client.GetFromJsonAsync<Message>($"messages/{index}");

            activity?.AddEvent(new("end cache invalidation"));

            return message;
        });
    }
}