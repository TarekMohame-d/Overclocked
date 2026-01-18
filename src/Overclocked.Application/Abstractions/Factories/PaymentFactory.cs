using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants.Payment;

namespace Overclocked.Application.Abstractions.Factories;

public class PaymentFactory(IEnumerable<IPaymentProviderService> providers)
{
    public IPaymentProviderService GetProvider(PaymentProvider provider) =>
        providers.FirstOrDefault(p => p.PaymentProvider == provider)
        ?? throw new Exception($"Provider {provider} is not implemented.");
}
