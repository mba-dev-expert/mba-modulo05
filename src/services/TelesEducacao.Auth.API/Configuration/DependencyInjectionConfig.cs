using MediatR;
using TelesEducacao.Auth.Application.Services;
using TelesEducacao.Core.Communication.Mediator;
using TelesEducacao.Core.Messages.CommomMessages.Notifications;
using TelesEducacao.WebAPI.Core.Usuario;

namespace TelesEducacao.Auth.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        //Notifications
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
    }
}
