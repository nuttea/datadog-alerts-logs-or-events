# Datadog API for Logs and Events

This is a .NET 8 REST API application for testing sending logs and events to Datadog.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A Datadog account and API key.

## Setup

1.  Clone this repository.
2.  Set the `DD_API_KEY` and `DD_APP_KEY` environment variables to your Datadog API and Application keys.
3.  (Optional) If you are not using the Datadog US1 site, set the `DD_SITE` environment variable. Common values are `datadoghq.com` (EU), `us3.datadoghq.com` (US3), `us5.datadoghq.com` (US5).

    For macOS/Linux:
    ```bash
    export DD_API_KEY="<YOUR_DATADOG_API_KEY>"
    export DD_APP_KEY="<YOUR_DATADOG_APP_KEY>"
    export DD_SITE="<YOUR_DATADOG_SITE>"
    ```

    For Windows (PowerShell):
    ```powershell
    $env:DD_API_KEY="<YOUR_DATADOG_API_KEY>"
    $env:DD_APP_KEY="<YOUR_DATADOG_APP_KEY>"
    $env:DD_SITE="<YOUR_DATADOG_SITE>"
    ```

4.  Build the project:

    ```bash
    dotnet build Datadog.Api
    ```

## Running the Application

Make sure the `DD_API_KEY` and `DD_APP_KEY` environment variables are set in your terminal session before running the application.

```bash
dotnet run --project Datadog.Api
```

The application will start on `http://localhost:5000`.

### Running in a Development Environment

The `app.Environment.IsDevelopment()` check in `Program.cs` is controlled by the `ASPNETCORE_ENVIRONMENT` environment variable. When you use `dotnet run`, this variable is automatically set to `Development` by the `Properties/launchSettings.json` file, which enables development features like the Swagger UI.

If you need to run the application outside of `dotnet run` (for example, in a production environment or a different IDE), you can explicitly set this variable:

For macOS/Linux:
```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --project Datadog.Api
```

For Windows (PowerShell):
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project Datadog.Api
```

## Swagger UI

Once the application is running in the Development environment, you can access the Swagger UI to view interactive API documentation and test the endpoints. Open your browser and navigate to:

[http://localhost:5000/swagger](http://localhost:5000/swagger)

## Endpoints

### Send a log message

-   **URL:** `/datadog/log`
-   **Method:** `POST`
-   **Body:**

    ```json
    {
      "message": {
        "title": "Online Document Service (General Agreement, Terms of Service) - English",
        "details": "The Online Document Service failed to process the request.",
        "status": "error",
        "transaction_id": "a3564212-5d98-4802-b4f8-9643da6c552a",
        "tags": {
          "event":"failed", 
          "app":"online-document"
        },
        "url": "https://httpbin.org/anything/a3564212-5d98-4802-b4f8-9643da6c552a"
      }
    }
    ```

-   **cURL example with random `aggregation_key`:**

    ```bash
    AGGREGATION_KEY=$(uuidgen) && curl -X POST -H "Content-Type: application/json" -d '{"message":{"title":"Online Document Service (General Agreement, Terms of Service) - English","details":"The Online Document Service failed to process the request.","status":"error","tags":{"event":"failed","app":"online-document"},"transaction_id":"'$AGGREGATION_KEY'","url":"https://httpbin.org/anything/'$AGGREGATION_KEY'"}}' http://localhost:5000/datadog/log
    ```

### Send an event

-   **URL:** `/datadog/event`
-   **Method:** `POST`
-   **Body:**

    ```json
    {
      "title": "Online Document Service (General Agreement, Terms of Service) - English",
      "message": "The Online Document Service failed to process the request. For more details, please check the transaction logs. [View Transaction Details](https://httpbin.org/anything/a3564212-5d98-4802-b4f8-9643da6c552a)",
      "priority": "normal",
      "status": "error",
      "tags": ["event:failed", "app:online-document"],
      "aggregation_key": "a3564212-5d98-4802-b4f8-9643da6c552a",
      "useMarkdown": true,
      "source_type_name": "online-document"
    }
    ```

-   **cURL example with random `aggregation_key`:**

    ```bash
    AGGREGATION_KEY=$(uuidgen) && curl -X POST -H "Content-Type: application/json" -d '{"title":"Online Document Service (General Agreement, Terms of Service) - English","message":"The Online Document Service failed to process the request. For more details, please check the transaction logs. [View Transaction Details](https://httpbin.org/anything/'$AGGREGATION_KEY')","priority":"normal","status":"error","tags":["event:failed","app:online-document"],"aggregation_key":"'$AGGREGATION_KEY'","useMarkdown":true,"source_type_name":"dotnet"}' http://localhost:5000/datadog/event
    ```
## Monitors

Example Datadog Monitors

- [Monitors from Event](../monitors/monitors-event.json)
- [Monitors from Log](../monitors/monitors-log.json)