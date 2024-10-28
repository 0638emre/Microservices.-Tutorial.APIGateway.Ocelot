var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "API 3 Running");

app.Run();