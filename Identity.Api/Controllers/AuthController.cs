using Identity.Api.DomainComponents;
using Identity.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace Identity.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IIdentityDomainComponent _identityDomainComponent;
    public AuthController(IIdentityDomainComponent identityDomainComponent)
    {
        _identityDomainComponent = identityDomainComponent;
    }
    [HttpPost("login")]
    [ProducesResponseType<LoginResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken ct)
    {
        var ua = Request.Headers["User-Agent"].ToString();
        var result = await _identityDomainComponent.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), ua, null, ct);
        return result is null ? Unauthorized() : Ok(result);
    }
}
