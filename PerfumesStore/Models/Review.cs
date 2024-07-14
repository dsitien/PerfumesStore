using System;
using System.Collections.Generic;

namespace PerfumesStore.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
    public string Status { get; set; } = null!;

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
