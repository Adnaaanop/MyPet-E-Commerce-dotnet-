namespace MyApp.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Placed";      //Placed, Shipped, Delivered, Cancelled
        public string Address { get; set; } = null!;        
        public decimal Total { get; set; }

        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
