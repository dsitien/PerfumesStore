using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.OrderManager
{
    public class IndexModel : PageModel
    {
        private readonly PerfumesStore.Models.PerfumesShopContext _context;

        public IndexModel(PerfumesStore.Models.PerfumesShopContext context)
        {
            _context = context;
        }
        public IList<Order> Order { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Order = await _context.Orders
                .Include(o => o.User).ToListAsync();
        }
    }
}
