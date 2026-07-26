using TelesEducacao.Pagamentos.API.Configuration;
using TelesEducacao.Pagamentos.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiConfigurations(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerConfigureServices();
builder.Services.RegisterServices();

var app = builder.Build();

app.Services.UseDbMigrationPagamentosHelper();

app.UseSwaggerConfiguration();
app.UseApiCoreConfigurations();

app.Run();
