namespace MyApp.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }     // required
        public int? ProductId { get; set; } // exactly one of ProductId / PetId
        public int? PetId { get; set; }
        public int Quantity { get; set; }

        public User User { get; set; } = null!;
        public Product? Product { get; set; }
        public Pet? Pet { get; set; }
    }
}
