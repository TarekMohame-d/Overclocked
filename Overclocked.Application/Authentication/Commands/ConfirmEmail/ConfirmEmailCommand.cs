using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Authentication.Commands.ConfirmEmail;

public record ConfirmEmailCommand : ICommand
{
    public required string Email { get; init; }
    public required string Code { get; init; }
}
