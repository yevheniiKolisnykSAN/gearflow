using MediatR;

namespace GearFlow.Application.Commands.Auth;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password) : IRequest<Boolean>;