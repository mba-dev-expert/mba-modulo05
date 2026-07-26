using TelesEducacao.Auth.API.Configuration;
using TelesEducacao.Auth.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfigurations(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerConfigureServices();
builder.Services.RegisterServices();

var app = builder.Build();

app.Services.UseDbMigrationAuthHelper();

app.UseSwaggerConfiguration();
app.UseApiCoreConfigurations();

app.Run();
