using System;
using System.Collections.Generic;

namespace PerfumesStore.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string? Fullname { get; set; }

    public string? ShippingAddress { get; set; }

    public string? Phone { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
