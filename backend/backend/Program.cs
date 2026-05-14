using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Repositories.Interfaces;
using backend.Repositories.Implementations;
using backend.Services;
using backend.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================================================
// ✅ REGISTER ALL REPOSITORIES (MISSING ONES INCLUDED)
// =========================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();           // <-- THIS WAS MISSING!
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// =========================================================
// ✅ REGISTER ALL SERVICES
// =========================================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();          // Now ICartRepository will be resolved
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<ReviewService>();

// =========================================================
// ✅ HELPERS
// =========================================================
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddSingleton<EmailHelper>();

// =========================================================
// ✅ AUTHENTICATION (JWT)
// =========================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// =========================================================
// ✅ CORS (Allow Angular)
// =========================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();




using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Users.Any(u => u.Email == "admin@test.com"))
    {
        var users = new[]
        {
            new backend.Models.User
            {
                FullName = "Test Admin",
                Email = "admin@test.com",
                PhoneNumber = "01000000003",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                WalletBalance = 0,
                RoleId = 3
            },
            new backend.Models.User
            {
                FullName = "Test Seller",
                Email = "seller@test.com",
                PhoneNumber = "01000000002",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                WalletBalance = 0,
                RoleId = 2
            },
            new backend.Models.User
            {
                FullName = "Test Customer",
                Email = "customer@test.com",
                PhoneNumber = "01000000001",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                WalletBalance = 0,
                RoleId = 1
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
        Console.WriteLine("✅ Test users seeded!");
    }
}

app.Run();