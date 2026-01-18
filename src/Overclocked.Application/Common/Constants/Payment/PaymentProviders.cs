namespace Overclocked.Application.Common.Constants.Payment;

public static class PaymentProviders
{
    public static List<(PaymentProvider Provider, List<PaymentMethod> Methods)> AllProviders => [Paymob, Internal];
    private static (PaymentProvider Provider, List<PaymentMethod> Methods) Paymob =>
        (PaymentProvider.Paymob, [PaymentMethod.CreditCard, PaymentMethod.EWallet]);

    private static (PaymentProvider Provider, List<PaymentMethod> Methods) Internal =>
        (PaymentProvider.Internal, [PaymentMethod.CashOnDelivery, PaymentMethod.Balance]);
}
