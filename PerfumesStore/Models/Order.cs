using System;
using System.Collections.Generic;

namespace PerfumesStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
