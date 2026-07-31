using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;



namespace ClassLibrary.Models
{
    public class MobiContext : IdentityDbContext<ApplicationUser>
    {
        public MobiContext(DbContextOptions<MobiContext> options) : base(options) { }
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Shop> Shops { get; set; } = null!;
        public DbSet<SellerLike> SellerLikes { get; set; } = null!;
        public DbSet<Logs> Logs { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<SliderImage> SliderImages { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var admin = new IdentityRole("admin") { NormalizedName = "ADMIN" };

            var client = new IdentityRole("client") { NormalizedName = "CLIENT" };

            var seller = new IdentityRole("seller") { NormalizedName = "SELLER" };

            modelBuilder.Entity<IdentityRole>().HasData(admin,client, seller);


            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Customer>(c => c.UserId);

            // رابطه یک به چند بین Customer و Address
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)    // یک مشتری به یک آدرس
                .WithMany(a => a.Customers) // یک آدرس به چند مشتری
                .HasForeignKey(c => c.AddressId) // تنظیم ForeignKey
                .OnDelete(DeleteBehavior.NoAction);  // تنظیم رفتار حذف اختیاری

            // تعریف رابطه بین Seller و Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Shop)
                .WithMany(s => s.products)
                .HasForeignKey(p => p.ShopId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProductId);

            // تعریف رابطه بین Order و Customer
            modelBuilder.Entity<Order>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            // تعریف رابطه بین Order و Product
            modelBuilder.Entity<Order>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            // ایندکس‌ها
            modelBuilder.Entity<Payment>().HasIndex(p => p.OrderId);
            modelBuilder.Entity<Payment>().HasIndex(p => p.PaymentDate);
            modelBuilder.Entity<Order>().HasIndex(o => o.CustomerId);

            modelBuilder.Entity<Cart>()
                .HasMany(x => x.Items)
                .WithOne(x => x.Cart)
                .HasForeignKey(x => x.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne(c => c.Cart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
