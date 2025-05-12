namespace Domain.Entities
{
    public class OrderStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }

        // Navigations
        public ICollection<Order> Orders { get; set; }
    }
}
