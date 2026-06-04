using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GearFlow.Application.Commands;
using GearFlow.Application.Commands.Auth;
using GearFlow.Application.DTOs;
using GearFlow.Application.Queries.Auth;
using GearFlow.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthController(IMediator mediator, IUserRepository UserRepository, IJwtService jwtService)
    {
        _mediator = mediator;
        _userRepository = UserRepository;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var token = await _mediator.Send(command);
        Response.Cookies.Append("AccessToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });


        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _mediator.Send(new GetCurrentUserQuery(id));

        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AccessToken");
        return Ok();
    }

    [HttpPost("microsoft")]
    public async Task<IActionResult> MicrosoftAuth([FromBody] MicrosoftAuthDto dto)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(dto.Token);
    
        var email = token.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == "preferred_username")?.Value;
    
        if (email == null || !email.EndsWith("@student.san.edu.pl"))
            return Unauthorized("Not a university account");
    
        var fullName = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "";
        var nameParts = fullName.Split(' ');
        var firstName = nameParts.Length > 1 ? nameParts[1] : fullName;
        var lastName = nameParts.Length > 0 ? nameParts[0] : "";
    
        var user = await _mediator.Send(new GetOrCreateMicrosoftUserCommand(email, firstName, lastName));
    
        var userWithRole = await _userRepository.GetByIdWithRoleAsync(user.Id);
    
        Response.Cookies.Append("AccessToken", _jwtService.GenerateJwtToken(userWithRole!), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    
        return Ok();
    }
}