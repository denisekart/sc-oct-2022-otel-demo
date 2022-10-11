using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMemoryCache()
    .AddTelemetry(c =>
    {
        c.ServiceName = "sc.otel.demo.service1";
        c.ServiceNamespace = "abcd";
        c.ServiceVersion = "1";
        c.OtlpExporterEndpoint = builder.Configuration.GetConnectionString("OtlpExporter");
    })
    .AddHttpClient("service2", client =>
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

app.MapPut("/messages", Endpoints.PutMessage)
    .WithName("PutMessage");
app.MapGet("/messages/{index:int}", Endpoints.GetMessage)
    .WithName("GetMessage");

app.Run();
