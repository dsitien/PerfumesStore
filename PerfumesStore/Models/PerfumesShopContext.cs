using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PerfumesStore.Models;

public partial class PerfumesShopContext : DbContext
{
    public PerfumesShopContext()
    {
    }

    public PerfumesShopContext(DbContextOptions<PerfumesShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        // Thêm dữ liệu mẫu cho tài khoản admin
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            Username = "admin",
            Password = "admin123@", // Lưu ý: Trong thực tế, bạn nên lưu trữ mật khẩu dưới dạng hash
            Email = "admin@example.com",
            FullName = "Admin User",
            Phone = "0123456789",
            Address = "123 Admin Street",
            Status = "Active",
            Role = "Admin"
        });

    }
}