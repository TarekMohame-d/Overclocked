using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Authentication.TestCases;

namespace Unit.Tests.Validations.Authentication;

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
        var request = new RefreshTokenRequest
        {
            AccessToken =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
                + ".eyJuYW1laWQibG9ja2VkQVBJIiwiYXVkIjoiT3ZlcmNsb2NrZWRVc2VycyJ9"
                + ".xsCFZ9E1iYatCccyPl-uBZa0qV3IjADKZ06FGNAeiU8",
            RefreshToken = refreshToken!,
        };

        // Act
        TestValidationResult<RefreshTokenRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken).Only();
    }
}
