# WORK IN PROGRESS

<a href="https://www.nuget.org/packages/zborek.LangfuseDotnet/">
 <img src="https://img.shields.io/nuget/v/zborek.LangfuseDotnet" alt="NuGet">
</a>

# Langfuse Example Web API

This repository contains library for Langfuse in .net

In default behaviour, application add background service for sending logs to Langfuse. You can disable it by setting `SendLogs` to `false` in `appsettings.json`.

```json
{
  "Langfuse": {
    "SendLogs": false
  }
}
```

## Getting Started

### Usage in Your Project

Inject `LangfuseClient` into your service and use it to send events to Langfuse.

For creating trace events use `LangfuseTrace`. Use can inject it into your service or create it.

You can use methods for creating observations:

```csharp
var trace = new LangfuseTrace("traceName", TimeProvider.System);
var span = trace.CreateSpan("GetDataFromDb");
var generation = trace.CreateGeneration("DbGeneration");
var event = trace.CreateEvent("DbEvent");

// or use from created span

var eventFromSpan = span.CreateEvent("DbEvent");
````

Logs can be viewed on https://cloud.langfuse.com/ or on local hosted Langfuse.



### Examples

1. Clone the repository:
    ```sh
    git clone https://github.com/your-repo/langfuse-example-webapi.git
    cd langfuse-example-webapi
    ```

2. Set api key in `appsettings.json`:
   ```json
   {
     "Langfuse": {
       "PublicKey": "",
       "SecretKey": ""
     },
     "OpenAI": {
       "ApiKey": ""
     }
   }
   ```

3. Restore dependencies:
    ```sh
    dotnet restore
    ```

### Running the Application

1. Build the project:
    ```sh
    dotnet build
    ```

2. Run the project:
    ```sh
    dotnet run --project Examples/Langfuse.Example.WebApi
    ```

### API Endpoints

- `POST /chat`: Processes a chat request and returns a response.
- `POST /chatDi`: Processes a chat request with dependency injection and returns a response.

### Contributing

Contributions are welcome! Please open an issue or submit a pull request.

### License

This project is licensed under the MIT License.