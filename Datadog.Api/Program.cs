using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Read Datadog API key from environment variable
var ddApiKey = Environment.GetEnvironmentVariable("DD_API_KEY");
if (!string.IsNullOrEmpty(ddApiKey))
{
    builder.Configuration["Datadog:ApiKey"] = ddApiKey;
    builder.Configuration["Serilog:WriteTo:1:Args:apiKey"] = ddApiKey;
}

var ddAppKey = Environment.GetEnvironmentVariable("DD_APP_KEY");
if (!string.IsNullOrEmpty(ddAppKey))
{
    builder.Configuration["Datadog:AppKey"] = ddAppKey;
}

var ddSite = Environment.GetEnvironmentVariable("DD_SITE");
if (!string.IsNullOrEmpty(ddSite))
{
    builder.Configuration["Datadog:Site"] = ddSite;
}

// Set up Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
