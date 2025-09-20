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
                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<CreateOrderRequest, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<OrderItem, OrderItemDto>();

            // Cart
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Name : null));
            CreateMap<AddCartItemRequest, CartItem>();

            // Wishlist
            CreateMap<WishlistItem, WishlistItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet != null ? src.Pet.Name : null));
            CreateMap<AddWishlistItemRequest, WishlistItem>();
        }
    }
}
