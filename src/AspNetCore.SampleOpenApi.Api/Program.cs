using AspNetCore.SampleOpenApi.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddConfiguredOpenApi(new Options {
        Title = "OpenApi Sample API",
        Description = "Sample API to test migration from Swashbuckle to AspNetCore OpenApi",
        DefaultVersionAssumedWhenUnspecified = false,
        Versions = [(1, 0), (2, 0)],
        DefaultVersion = (1, 0),
    })
    .AddConfiguredControllers()
    .AddConfiguredMinimalApi()
    .AddProblemDetails()
    .AddExceptionHandler<DefaultExceptionHandler>();

WebApplication app = builder.Build();

app
    .UseConfiguredHttps()
    .UseConfiguredExceptionHandler()
    .UseConfiguredAuthorization()
    .MapConfiguredOpenApi()
    .MapConfiguredControllers();

await app.RunAsync();
