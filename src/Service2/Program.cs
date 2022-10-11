using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MessagesContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTelemetry(c =>
{
    c.ServiceName = "sc.otel.demo.service2";
    c.ServiceNamespace = "efgh";
    c.ServiceVersion = "1";
    c.OtlpExporterEndpoint = builder.Configuration.GetConnectionString("OtlpExporter");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Text("Service 2 ready!"));
    using var serviceScope = app.Services.CreateScope();
    serviceScope.ServiceProvider.GetRequiredService<MessagesContext>().EnsureCreated();
}

app.MapPut("/messages", async ([FromQuery] string message, Meter meter, ILogger<Program> logger, MessagesContext database) =>
{
    var counterName = $"count_{message}";
    logger.LogWarning("Increased observability platform cost detected due to custom metric {metricName} reported", counterName);

    using (var scope = logger.BeginScope(new { scope = "Creating a counter", time = DateTime.Now }))
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

}).WithName("PutMessage");


app.MapGet("/messages/{index:int}", async (int index, MessagesContext database) =>
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

}).WithName("GetMessage");

app.Run();
