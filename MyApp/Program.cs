using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApp.Data;
using MyApp.Helpers;
using MyApp.Middlewares;
using MyApp.Repositories;
using MyApp.Repositories.Interfaces;
using MyApp.Services;
using MyApp.Services.Interfaces;
using System.Text;
using Microsoft.OpenApi.Models;

namespace MyApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };

                // Read JWT from cookie
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["accessToken"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IPetRepository, PetRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IPetService, PetService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            // Cloudinary
            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<CloudinaryService>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // CORS for React with credentials
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Cookie policy (local dev)
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // false for local HTTP
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None; // allow cross-origin
            });

            builder.Services.AddEndpointsApiExplorer();
            // Inside Main method, replace the existing builder.Services.AddSwaggerGen() with:
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApp API", Version = "v1" });

                // Add JWT Authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the JWT token (e.g., 'Bearer eyJ...'). For cookie-based auth, use the 'accessToken' cookie value here."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });

            var app = builder.Build();

            // Run migrations and seed DB
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbInitializer.InitializeAsync(services);
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.MapControllers();
            app.Run();
        }
    }
}