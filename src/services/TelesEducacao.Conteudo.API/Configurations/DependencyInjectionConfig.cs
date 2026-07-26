using FluentValidation.Results;
using MediatR;
using TelesEducacao.Conteudo.Application.Commands;
using TelesEducacao.Conteudo.Application.Services;
using TelesEducacao.Conteudo.Data;
using TelesEducacao.Conteudo.Data.Repository;
using TelesEducacao.Conteudo.Domain;
using TelesEducacao.Core.Communication.Mediator;
using TelesEducacao.Core.Messages.CommomMessages.Notifications;

namespace TelesEducacao.Conteudo.API.Configurations;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<ICargaHorariaService, CargaHorariaService>();
        services.AddScoped<ICursoRepository, CursoRepository>();
        services.AddScoped<ICursoAppService, CursoAppService>();
        services.AddScoped<ConteudosContext>();

        services.AddScoped<IRequestHandler<CriarCursoCommand, bool>, CriarCursoCommandHandler>();
        services.AddScoped<IRequestHandler<CriarAulaCommand, bool>, CriarAulaCommandHandler>();

        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
        services.AddScoped<IMediatorHandler, MediatorHandler>();
    }
}
