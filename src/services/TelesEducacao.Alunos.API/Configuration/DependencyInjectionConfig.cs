using MediatR;
using TelesEducacao.Alunos.Application.Commands;
using TelesEducacao.Alunos.Application.Queries;
using TelesEducacao.Alunos.Data.Repository;
using TelesEducacao.Alunos.Domain;
using TelesEducacao.Core.Communication.Mediator;
using TelesEducacao.Core.Messages.CommomMessages.Notifications;

namespace TelesEducacao.Alunos.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IAlunoRepository, AlunoRepository>();
        services.AddScoped<IAlunoQueries, AlunoQueries>();
        services.AddScoped<IRequestHandler<CriarAlunoCommand, bool>, CriarAlunoCommandHandler>();
        services.AddScoped<IRequestHandler<AdicionarMatriculaCommand, bool>, AdicionarMatriculaCommandHandler>();
        services.AddScoped<IRequestHandler<ConcluirAulaCommand, bool>, ConcluirAulaCommandHandler>();
        services.AddScoped<IRequestHandler<ConcluirCursoCommand, bool>, ConcluirCursoCommandHandler>();
        //Notifications
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
    }
}
