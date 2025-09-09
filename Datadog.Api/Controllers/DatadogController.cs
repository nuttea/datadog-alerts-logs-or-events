using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

namespace Datadog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DatadogController : ControllerBase
{
    private readonly ILogger<DatadogController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public DatadogController(ILogger<DatadogController> logger, IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    [HttpPost("log")]
    public IActionResult PostLog([FromBody] LogMessage message)
    {
        _logger.LogInformation("Received log message: {Message}", JsonSerializer.Serialize(message.Message));
        Log.Information(JsonSerializer.Serialize(message.Message));
        return Ok("Log message sent.");
    }

    [HttpPost("event")]
    public async Task<IActionResult> PostEvent([FromBody] EventPayload payload)
    {
        var apiKey = _configuration["Datadog:ApiKey"];
        var appKey = _configuration["Datadog:AppKey"];
        var site = _configuration["Datadog:Site"] ?? "datadoghq.com";

        _logger.LogInformation("Attempting to send event to Datadog using API v1.");
        _logger.LogInformation("DD_API_KEY loaded: {IsLoaded}", !string.IsNullOrEmpty(apiKey));
        _logger.LogInformation("DD_APP_KEY loaded: {IsLoaded}", !string.IsNullOrEmpty(appKey));
        _logger.LogInformation("DD_SITE: {Site}", site);

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Datadog API key is not configured.");
            return StatusCode(500, "Datadog API key is not configured.");
        }

        var eventText = payload.UseMarkdown && !string.IsNullOrEmpty(payload.Message)
            ? $"%%% \n{payload.Message}\n %%%"
            : payload.Message;

        var datadogEvent = new DatadogEventV1
        {
            Title = payload.Title,
            Text = eventText,
            Tags = payload.Tags,
            Priority = payload.Priority,
            AlertType = payload.Status,
            AggregationKey = payload.AggregationKey,
            DateHappened = payload.DateHappened ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            SourceTypeName = payload.SourceTypeName ?? Assembly.GetExecutingAssembly().GetName().Name
        };

        var client = _clientFactory.CreateClient();
        var apiUrl = $"https://api.{site}/api/v1/events";
        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
        request.Headers.Add("DD-API-KEY", apiKey);
        
        if (!string.IsNullOrEmpty(appKey))
        {
            request.Headers.Add("DD-APPLICATION-KEY", appKey);
        }

        var jsonPayload = JsonSerializer.Serialize(datadogEvent, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Event successfully sent to Datadog.");
            return Ok("Event sent.");
        }
        else
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send event to Datadog. Status Code: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
            return StatusCode((int)response.StatusCode, responseContent);
        }
    }
}

public class LogMessage
{
    public object? Message { get; set; }
}

public class EventPayload
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("aggregation_key")]
    public string? AggregationKey { get; set; }

    [JsonPropertyName("useMarkdown")]
    public bool UseMarkdown { get; set; }

    [JsonPropertyName("date_happened")]
    public long? DateHappened { get; set; }

    [JsonPropertyName("source_type_name")]
    public string? SourceTypeName { get; set; }
}

public class DatadogEventV1
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("alert_type")]
    public string? AlertType { get; set; }

    [JsonPropertyName("aggregation_key")]
    public string? AggregationKey { get; set; }

    [JsonPropertyName("date_happened")]
    public long? DateHappened { get; set; }

    [JsonPropertyName("source_type_name")]
    public string? SourceTypeName { get; set; }
}
