var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IConfiguration>(configuration);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
