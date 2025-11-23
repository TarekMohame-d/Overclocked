using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Authentication.Helpers.Interfaces;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService(
    IUserRepository userRepository,
    IRolePermissionsRepository rolePermissionsRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEventDispatcher eventDispatcher,
    IEmailConfirmationCodeService emailConfirmationCodeService,
    ITokenProvider tokenProvider,
    IRefreshTokenService refreshTokenService,
    ITokenReaderService tokenReaderService,
    ICartService cartService,
    IWishlistService wishlistService)
    : IAuthenticationService;
