using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Abstraction.Services;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands(
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
    ITokenProvider tokenProvider,
    IRefreshTokenHasher refreshTokenHasher,
    IPasswordHasher passwordHasher,
    IEmailConfirmationCodeService emailConfirmationCodeService,
    ITokenReaderService tokenReaderService,
    IUnitOfWork unitOfWork) : IAuthenticationCommands;
