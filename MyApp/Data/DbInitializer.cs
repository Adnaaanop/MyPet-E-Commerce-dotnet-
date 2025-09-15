using Microsoft.EntityFrameworkCore;
using MyApp.Entities;

namespace MyApp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply pending migrations
            await ctx.Database.MigrateAsync();

            // Seed only if empty
            if (!await ctx.Users.AnyAsync())
            {
                // temporary plain hash placeholder; 
                var admin = new User
                {
                    Name = "Super Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = "admin123_hashed_placeholder",
                    Role = "Admin",
                    IsActive = true
                };
                ctx.Users.Add(admin);
            }

            if (!await ctx.Products.AnyAsync())
            {
                ctx.Products.AddRange(
                    new Product
                    {
                        Name = "Pedigree Adult Dog Food",
                        Category = "Dog",
                        Price = 430,
                        Description = "Complete and balanced dog food for adult dogs.",
                        Rating = 4.5,
                        Stock = 991,
                        ImageUrl = "https://plus.unsplash.com/premium_photo-1683134386510-ea7b2395fb37"
                    },
                    new Product
                    {
                        Name = "Whiskas Dry Cat Food",
                        Category = "Cat",
                        Price = 400,
                        Description = "Tasty and nutritious cat food for healthy fur.",
                        Rating = 4.2,
                        Stock = 181,
                        ImageUrl = "https://images.unsplash.com/photo-1592194996308-7b43878e84a6"
                    }
                );
            }

            if (!await ctx.Pets.AnyAsync())
            {
                ctx.Pets.AddRange(
                    new Pet
                    {
                        Name = "Buddy",
                        Breed = "Golden Retriever",
                        Age = 2,
                        Price = 15000,
                        Description = "Good Dog",
                        ImageUrl = "https://images.unsplash.com/photo-1583511655826-05700d52f4d9",
                        Category = "Dog",
                        Stock = 50
                    },
                    new Pet
                    {
                        Name = "Polly",
                        Breed = "Macaw Parrot",
                        Age = 3,
                        Price = 8000,
                        Description = "Naughty Parrot",
                        ImageUrl = "https://images.unsplash.com/photo-1680749512095-039b69a23ad2",
                        Category = "Bird",
                        Stock = 20
                    }
                );
            }

            await ctx.SaveChangesAsync();
        }
    }
}
