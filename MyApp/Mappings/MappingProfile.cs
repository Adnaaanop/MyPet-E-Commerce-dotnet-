using MyApp.DTOs.Cart;
using AutoMapper;
using MyApp.DTOs.Orders;
using MyApp.DTOs.Pets;
using MyApp.DTOs.Products;
using MyApp.DTOs.Users;
using MyApp.DTOs.Wishlist;
using MyApp.Entities;

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
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UpdateUserDto, User>();

            // Orders
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

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
