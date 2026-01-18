using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.PaymentUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.PaymentUseCases.GetPayments;

public class GetPaymentsRequestHandler : IRequestHandler<GetPaymentsRequest, PaymentResponse>
{
    public Task<Result<PaymentResponse>> Handle(GetPaymentsRequest request, CancellationToken ct)
    {
        List<(PaymentProvider Provider, List<PaymentMethod> Methods)> payments = PaymentProviders.AllProviders;

        var result = new PaymentResponse
        {
            Payments = payments.ToDictionary(x => x.Provider.ToString(), x => x.Methods.ConvertAll(m => m.ToString())),
        };

        return Task.FromResult(Result.Success(result));
    }
}
