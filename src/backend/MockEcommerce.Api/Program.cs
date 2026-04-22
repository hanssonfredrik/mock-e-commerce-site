using MockEcommerce.Api.Endpoints;
using MockEcommerce.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IProductService, MockProductService>();
builder.Services.AddSingleton<ICartService, InMemoryCartService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapOpenApi();
app.MapProductEndpoints();
app.MapCartEndpoints();

app.Run();

/// <summary>Enables <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}"/> in integration tests.</summary>
public partial class Program { }
