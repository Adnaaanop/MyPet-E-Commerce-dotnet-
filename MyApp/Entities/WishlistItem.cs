namespace MyApp.Entities
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ProductId { get; set; }
        public int? PetId { get; set; }

        public User User { get; set; } = null!;
        public Product? Product { get; set; }
        public Pet? Pet { get; set; }
    }
}
