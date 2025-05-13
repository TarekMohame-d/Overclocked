namespace Domain.Entities
{
    public class PaymentStatus
    {
        public int PaymentStatusId { get; set; }
        public string PaymentStatusName { get; set; }

        // Navigations
        public ICollection<Payment> Payments { get; set; }
    }
}
