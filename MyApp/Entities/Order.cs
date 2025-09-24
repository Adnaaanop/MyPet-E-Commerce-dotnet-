using System.ComponentModel.DataAnnotations;
using MyApp.Entities;

namespace MyApp.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Placed; // Using enum
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

        // Payment details
        public string? PaymentId { get; set; } // Razorpay payment ID
        public string? PaymentStatus { get; set; } // e.g., "paid", "failed"

        // Address
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;

        public User User { get; set; } = null!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}