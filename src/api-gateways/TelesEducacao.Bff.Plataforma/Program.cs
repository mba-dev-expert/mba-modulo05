using TelesEducacao.Bff.Plataforma.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfigurations(builder.Configuration);
builder.Services.AddHttpClientsConfiguration();
builder.Services.AddSwaggerConfigureServices();
builder.Services.RegisterServices();

var app = builder.Build();

app.UseApiCoreConfigurations();

app.Run();
