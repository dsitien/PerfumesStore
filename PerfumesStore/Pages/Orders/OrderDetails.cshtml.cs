using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;
using System.Linq;

namespace PerfumesStore.Pages.Orders
{
    public class OrderDetails : PageModel
    {

        private readonly CartService _cartService;
        private readonly PerfumesShopContext _context;

        public OrderDetails(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }
        [BindProperty]
        public Order Order { get; set; } = default!;
        public string message { get; set; }
        public User CurrentUsers { get; set; }
        [BindProperty]
        public List<OrderDetail> Orderdetails { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            if (currentUser != null)
            {
                CurrentUsers = currentUser;
                message = currentUser.FullName;
            }
            var selectedCartIds = HttpContext.Session.GetObject<List<int>>("SelectedCartIds") ?? new List<int>();
            if (currentUser == null)
            {
                return RedirectToPage("/Login/login");
            }

            Order = await _context.Orders.Include(p => p.User)
                .FirstOrDefaultAsync(a => a.UserId.Equals(currentUser.UserId) && a.OrderId.Equals(id));
            if (Order == null)
            {
                return NotFound();
            }
            // Fetch order details for the specified order ID
            Orderdetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == id)
                .ToListAsync();

            return Page();
        }
    }
}
