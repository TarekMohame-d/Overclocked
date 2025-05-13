namespace Domain.Entities
{
    public class OrderStatus
    {
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }

        // Navigations
        public ICollection<Order> Orders { get; set; }
    }
}
