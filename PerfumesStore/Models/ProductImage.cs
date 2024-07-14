using System;
using System.Collections.Generic;

namespace PerfumesStore.Models;

public partial class ProductImage
{
    public int ImageId { get; set; }

    public int? ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
