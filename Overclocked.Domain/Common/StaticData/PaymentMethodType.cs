namespace Overclocked.Domain.Common.StaticData;

public enum PaymentMethodType
{
    CreditCard = 1, // Standard online payment using Visa, MasterCard, or Amex.
    EWallet, // Payment using a mobile wallet.
    Cash, // Customer pays in cash when the order is delivered.
}
