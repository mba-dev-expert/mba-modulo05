using TelesEducacao.Conteudo.API.Configuration;
using TelesEducacao.Conteudo.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiConfigurations(builder.Configuration, builder.Environment);
builder.Services.AddSwaggerConfigureServices();
builder.Services.RegisterServices();

var app = builder.Build();
app.Services.UseDbMigrationConteudosHelper();
app.UseSwaggerConfiguration();
app.UseApiCoreConfigurations();

app.Run();