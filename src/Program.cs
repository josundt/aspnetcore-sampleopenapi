WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    // .AddProblemDetails(o => o.CustomizeProblemDetails = ctx => {
    //     ctx.
    // })
    .AddCustomOpenApi(new Options {
        Title = "OpenApi Sample API",
        Description = "Sample API to test migration from Swashbuckle to AspNetCore OpenApi",
        DefaultVersionAssumedWhenUnspecified = false,
        Versions = [(1, 0), (2, 0)],
        DefaultVersion = (1, 0),
    })
    .AddCustomControllers();

WebApplication app = builder.Build();

app
    .UseCustomHttps()
    .UseCustomExceptionHandler()
    .UseCustomAuthorization()
    .MapCustomOpenApi()
    .UseCustomScalar()
    .MapCustomControllers();

await app.RunAsync();
