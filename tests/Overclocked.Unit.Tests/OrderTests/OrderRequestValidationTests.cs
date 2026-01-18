using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.OrderUseCases.CreateOrder;
using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
using Overclocked.Application.Features.OrderUseCases.GetPagedOrders;
using Shouldly;

namespace Overclocked.Unit.Tests.OrderTests;

public class OrderRequestValidationTests
{
    [Fact]
    public void CreateOrderRequest_FromDto_ShouldReturnFailure_WhenPaymentMethodIsInvalid()
    {
        // Arrange
        var dto = CreateDto(paymentMethod: "InvalidMethod");

        // Act
        var result = CreateOrderRequest.FromDto(dto, Guid.NewGuid());

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Description.ShouldContain("Invalid Payment Method Type");
    }

    [Fact]
    public void CreateOrderRequest_FromDto_ShouldReturnFailure_WhenPaymentProviderIsInvalid()
    {
        // Arrange
        var dto = CreateDto(paymentProvider: "InvalidProvider", paymentMethod: "CreditCard");

        // Act
        var result = CreateOrderRequest.FromDto(dto, Guid.NewGuid());

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Description.ShouldContain("Invalid Payment Provider Type");
    }

    [Fact]
    public void CreateOrderRequest_FromDto_ShouldReturnFailure_WhenProviderDoesNotSupportMethod()
    {
        // Arrange
        // Paymob does not support Balance (Internal does)
        var dto = CreateDto(paymentProvider: "Paymob", paymentMethod: "Balance");

        // Act
        var result = CreateOrderRequest.FromDto(dto, Guid.NewGuid());

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Description.ShouldContain("does not support payment method");
    }

    [Fact]
    public void CreateOrderRequestValidator_ShouldHaveErrors_WhenAddressIsInvalid()
    {
        // Arrange
        var validator = new CreateOrderRequestValidator();
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(0, "", "", "", "", ""),
            PaymentMethod = PaymentMethod.CreditCard,
            PaymentProvider = PaymentProvider.Paymob
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName.Contains("Building"));
        result.Errors.ShouldContain(e => e.PropertyName.Contains("Apartment"));
    }

    [Fact]
    public void GetPagedOrdersRequestValidator_ShouldHaveErrors_WhenRequestIsInvalid()
    {
        // Arrange
        var validator = new GetPagedOrdersRequestValidator();
        var request = new GetPagedOrdersRequest { UserId = Guid.NewGuid(), Page = 0, PageSize = 101, Year = 2000 };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Page");
        result.Errors.ShouldContain(e => e.PropertyName == "PageSize");
        result.Errors.ShouldContain(e => e.PropertyName == "Year");
    }

    private CreateOrderRequestDto CreateDto(string paymentMethod = "CreditCard", string paymentProvider = "Paymob")
    {
        return new CreateOrderRequestDto(
            new ShippingAddressRequestDto(1, "B", "S", "C", "1", "D"),
            paymentMethod,
            paymentProvider
        );
    }
}
