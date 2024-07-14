using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumesStore.Models
{
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100, ErrorMessage = "Product Name cannot be longer than 100 characters.")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Product Image is required.")]
        public byte[] ProductImg { get; set; } = null!;

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot be longer than 20 characters.")]
        public string Status { get; set; } = null!;

        public virtual Brand? Brand { get; set; }

        public virtual Category? Category { get; set; }

        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}