using Microsoft.EntityFrameworkCore;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddTelemetry(c =>
    {
        c.ServiceName = "sc.otel.demo.service2";
        c.ServiceNamespace = "efgh";
        c.ServiceVersion = "1";
        c.OtlpExporterEndpoint = builder.Configuration.GetConnectionString("OtlpExporter");
    })
    .AddDbContext<MessagesContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", Endpoints.Ready);
    using var serviceScope = app.Services.CreateScope();
    serviceScope.ServiceProvider.GetRequiredService<MessagesContext>().EnsureCreated();
}

app.MapPut("/messages", Endpoints.PutMessage)
    .WithName("PutMessage");
app.MapGet("/messages/{index:int}", Endpoints.GetMessage)
    .WithName("GetMessage");

app.Run();
