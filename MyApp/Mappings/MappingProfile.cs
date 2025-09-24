using AutoMapper;
using MyApp.DTOs.Cart;
using MyApp.DTOs.Orders;
using MyApp.DTOs.Pets;
using MyApp.DTOs.Products;
using MyApp.DTOs.Users;
using MyApp.DTOs.Wishlist;
using MyApp.Entities;
using MyApp.Enums;

namespace MyApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Pets
            CreateMap<Pet, PetDto>().ReverseMap();
            CreateMap<CreatePetDto, Pet>();
            CreateMap<UpdatePetDto, Pet>();

            // Products
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // Users
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role == "Admin" ? UserRole.Admin : UserRole.User))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? UserStatus.Active : UserStatus.Blocked));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role == UserRole.Admin ? "Admin" : "User"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == UserStatus.Active));

            // Orders
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ReverseMap(); // Add ReverseMap for OrderDto to Order

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<OrderItem, OrderItemDto>();

            // Cart
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Name : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : (src.Pet != null ? src.Pet.Price : 0)))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : (src.Pet != null ? src.Pet.ImageUrl : null)))
                .ForMember(dest => dest.Breed, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Breed : null))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Age : (int?)null));

            // Wishlist
            CreateMap<WishlistItem, WishlistItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Name : null))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : (src.Pet != null ? src.Pet.ImageUrl : null)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : (src.Pet != null ? src.Pet.Price : 0)))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Product != null ? src.Product.Rating : (double?)null))
                .ForMember(dest => dest.Breed, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Breed : null))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Age : (int?)null));
        }
    }
}