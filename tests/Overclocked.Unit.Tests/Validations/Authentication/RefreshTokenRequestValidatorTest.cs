using FluentValidation.TestHelper;
using Overclocked.Application.Features.AuthenticationUseCases.RefreshToken;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class RefreshTokenRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(RefreshTokenValidationTestCases.InvalidAccessTokenCases),
        MemberType = typeof(RefreshTokenValidationTestCases)
    )]
    public void RefreshTokenRequestValidator_Should_ReturnError_When_AccessTokenIsInvalid(string? accessToken)
    {
        // Arrange
        var validator = new RefreshTokenRequestValidator();
        var request = new RefreshTokenRequest { AccessToken = accessToken!, RefreshToken = "refresh-token" };

        // Act
        TestValidationResult<RefreshTokenRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.AccessToken).Only();
    }

    [Theory]
    [MemberData(
        nameof(RefreshTokenValidationTestCases.InvalidRefreshTokenCases),
        MemberType = typeof(RefreshTokenValidationTestCases)
    )]
    public void RefreshTokenRequestValidator_Should_ReturnError_When_RefreshTokenIsInvalid(string? refreshToken)
    {
        // Arrange
        var validator = new RefreshTokenRequestValidator();
        var accessToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
            + ".eyJuYW1laWQibG9ja2VkQVBJIiwiYXVkIjoiT3ZlcmNsb2NrZWRVc2VycyJ9"
            + ".xsCFZ9E1iYatCccyPl-uBZa0qV3IjADKZ06FGNAeiU8";

        var request = new RefreshTokenRequest { AccessToken = accessToken, RefreshToken = refreshToken! };

        // Act
        TestValidationResult<RefreshTokenRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken).Only();
    }
}
