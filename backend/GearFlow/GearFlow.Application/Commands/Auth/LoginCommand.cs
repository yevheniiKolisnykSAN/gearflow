using MediatR;

namespace GearFlow.Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<string>;