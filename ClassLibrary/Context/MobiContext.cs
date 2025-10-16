using ClassLibrary.Models;
using ClassLibrary.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class MobiContext : IdentityDbContext<IdentityUser>
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var admin = new IdentityRole("admin") { NormalizedName = "ADMIN" };

            var client = new IdentityRole("client") { NormalizedName = "CLIENT" };

            var seller = new IdentityRole("seller") { NormalizedName = "SELLER" };

            modelBuilder.Entity<IdentityRole>().HasData(admin,client, seller);

            // رابطه یک به چند بین Customer و Address
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)    // یک مشتری به یک آدرس
                .WithMany(a => a.Customers) // یک آدرس به چند مشتری
                .HasForeignKey(c => c.AddressId) // تنظیم ForeignKey
                .OnDelete(DeleteBehavior.NoAction);  // تنظیم رفتار حذف اختیاری

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.products)
                .WithMany(p => p.customers)
                .UsingEntity<Dictionary<string, object>>(
                "ProductCustomer",  // table name
            right => right
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.NoAction),
            left => left
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey("CustomerId")
                .OnDelete(DeleteBehavior.NoAction),
            join =>
            {
                join.HasKey("CustomerId", "ProductId"); // composite PK
                join.ToTable("ProductCustomer");
            });

            // تعریف رابطه بین Seller و Product
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.products)
                .WithMany(p => p.sellers)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductSeller",
            right => right
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.NoAction),
            left => left
                .HasOne<Shop>()
                .WithMany()
                .HasForeignKey("SellerId")
                .OnDelete(DeleteBehavior.NoAction),
            join =>
            {
                join.HasKey("SellerId", "ProductId");
                join.ToTable("ProductSeller");
            });


            // تعریف رابطه بین Sale و Customer
            modelBuilder.Entity<Order>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);

            // تعریف رابطه بین Sale و Product
            modelBuilder.Entity<Order>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            //// Ensure that ProductType is stored as an integer in the database
            //modelBuilder.Entity<Product>()
            //    .Property(p => p.Pro)
            //    .HasConversion<int>();  // This will store the enum as an integer (e.g., 0, 1, 2)
        }
    }
}
