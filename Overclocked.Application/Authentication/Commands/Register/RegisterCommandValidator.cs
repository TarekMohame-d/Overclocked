using FluentValidation;
using Overclocked.Application.Abstractions.Persistence;

namespace Overclocked.Application.Authentication.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private readonly IUserRepository _userRepository;
    public RegisterCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
            .WithMessage("{PropertyName} is not valid email address.")
            .MaximumLength(100)
            .WithMessage("{PropertyName} must be at most 100 characters long.")
            .MustAsync(async (email, cancellation) =>
            {
                var exists = await _userRepository.AnyAsync(x => x.Email == email, cancellation);

                return !exists;
            })
            .WithMessage("{PropertyName} already exists, please use another email or login.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MinimumLength(8)
            .WithMessage("{PropertyName} must be at least 8 characters long.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""':{}|<>]).{8,}$")
            .WithMessage("{PropertyName} must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must be at most 20 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must be at most 20 characters long.");

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("{PropertyName} must be a valid phone number.")
            .MaximumLength(20)
            .WithMessage("{PropertyName} must be at most 20 characters long.")
            .MustAsync(async (phone, cancellation) =>
            {
                var exists = await _userRepository.AnyAsync(x => x.Phone == phone, cancellation);

                return !exists;
            })
            .WithMessage("{PropertyName} already exists, please use another phone number.");
    }
}
