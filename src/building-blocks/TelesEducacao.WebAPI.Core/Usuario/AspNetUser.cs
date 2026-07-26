using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TelesEducacao.WebAPI.Core.Usuario;

public class AspNetUser : IAspNetUser
{
    private readonly IHttpContextAccessor _accessor;

    public AspNetUser(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private HttpContext HttpContext => _accessor.HttpContext
                                       ?? throw new InvalidOperationException(
                                           "Não há um HttpContext ativo. Este serviço só pode ser usado no escopo de uma requisição HTTP.");

    public string? Name => HttpContext.User.Identity?.Name;

    public Guid ObterUserId()
    {
        if (!EstaAutenticado()) return Guid.Empty;

        var userId = HttpContext.User.GetUserId();

        return string.IsNullOrEmpty(userId)
            ? throw new InvalidOperationException("O usuário autenticado não possui a claim de identificação (NameIdentifier).")
            : Guid.Parse(userId);
    }

    public string ObterUserEmail()
    {
        return EstaAutenticado() ? HttpContext.User.GetUserEmail() ?? "" : "";
    }

    public string ObterUserToken()
    {
        return EstaAutenticado() ? HttpContext.User.GetUserToken() ?? "" : "";
    }

    public string ObterUserRefreshToken()
    {
        return EstaAutenticado() ? HttpContext.User.GetUserRefreshToken() ?? "" : "";
    }

    public bool EstaAutenticado()
    {
        return HttpContext.User.Identity?.IsAuthenticated ?? false;
    }

    public bool PossuiRole(string role)
    {
        return HttpContext.User.IsInRole(role);
    }

    public IEnumerable<Claim> ObterClaims()
    {
        return HttpContext.User.Claims;
    }

    public HttpContext ObterHttpContext()
    {
        return HttpContext;
    }
}