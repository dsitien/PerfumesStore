using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PerfumesStore.Models;

public partial class Brand
{
    [Key]
    public int BrandId { get; set; }

    [Required(ErrorMessage = "Brand Name is required.")]
    [StringLength(50, ErrorMessage = "Brand Name cannot be longer than 50 characters.")]
    public string BrandName { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(20, ErrorMessage = "Status cannot be longer than 20 characters.")]
    public string Status { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}