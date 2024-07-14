using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumesStore.Models;

public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Category Name is required.")]
    [StringLength(50, ErrorMessage = "Category Name cannot be longer than 50 characters.")]
    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(20, ErrorMessage = "Status cannot be longer than 20 characters.")]
    public string Status { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}