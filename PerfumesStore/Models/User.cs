using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumesStore.Models
{
    public partial class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = null!;

        public string Role { get; set; } = null!;

        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}