using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shared;
using System.Diagnostics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddTelemetry(c =>
{
    c.ServiceName = "sc.otel.demo.service1";
    c.ServiceNamespace = "abcd";
    c.ServiceVersion = "1";
    c.OtlpExporterEndpoint = builder.Configuration.GetConnectionString("OtlpExporter");
});

builder.Services.AddHttpClient("service2", client =>
{
    string service2Url = builder.Configuration.GetConnectionString("Service2")
                        ?? throw new ArgumentNullException("ConnectionsStrings/Service2");
    client.BaseAddress = new Uri(service2Url);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPut("/messages", async ([FromQuery] string message, IHttpClientFactory clientFactory, ILogger<Program> logger) =>
{
    using var client = clientFactory.CreateClient("service2");
    await client.PutAsync($"messages?message={message}", JsonContent.Create(new { }));

    logger.LogInformation("Posted message {message} to {service}", message, "/service2");

}).WithName("PutMessage");

app.MapGet("/messages/{index:int}", (int index, IHttpClientFactory clientFactory, ActivitySource activitySource, IMemoryCache cache, Meter meter) =>
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

}).WithName("GetMessage");

app.Run();
