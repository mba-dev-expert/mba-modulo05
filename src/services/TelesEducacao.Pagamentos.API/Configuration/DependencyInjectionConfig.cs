using TelesEducacao.Core.Communication.Mediator;
using TelesEducacao.Pagamentos.AntiCorruption;
using TelesEducacao.Pagamentos.Business;
using TelesEducacao.Pagamentos.Data.Repository;

// aliases pra evitar conflito com IConfigurationManager do .NET
using IPagamentosConfigManager = TelesEducacao.Pagamentos.AntiCorruption.IConfigurationManager;
using PagamentosConfigManager = TelesEducacao.Pagamentos.AntiCorruption.ConfigurationManager;

namespace TelesEducacao.Pagamentos.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IPagamentoCartaoCreditoFacade, PagamentoCartaoCreditoFacade>();
        services.AddScoped<IPayPalGateway, PayPalGateway>();
        services.AddSingleton<IPagamentosConfigManager, PagamentosConfigManager>();

        services.AddScoped<IPagamentoRepository, PagamentoRepository>();

        services.AddScoped<IMediatorHandler, MediatorHandler>();
    }
}
