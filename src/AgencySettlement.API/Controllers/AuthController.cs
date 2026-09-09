using AgencySettlement.Application.Features.Commands.LoginCommand;
using AgencySettlement.Application.Features.Commands.RegisterCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgencySettlement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterCommand(
                request.PhoneNumber,
                request.Password,
                request.FirstName,
                request.LastName),
            cancellationToken);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginCommand(
                request.PhoneNumber,
                request.Password),
            cancellationToken);

        return Ok(result);
    }
}

public sealed record RegisterRequest(
    string PhoneNumber,
    string Password,
    string FirstName,
    string LastName);

public sealed record LoginRequest(
    string PhoneNumber,
    string Password);