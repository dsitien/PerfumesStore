using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages.Orders
{
    public class ViewOrdersModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly PerfumesShopContext _context;

        public ViewOrdersModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [BindProperty]
        public List<Order> Orders { get; set; } = default!;
        [BindProperty]
        public User CurrentUsers { get; set; }
        public string message { get; set; }
        [BindProperty]
        public List<OrderDetail> Orderdetails { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            if (currentUser == null)
            {
                return RedirectToPage("/Login/login");
            }
            if (currentUser != null)
            {
                CurrentUsers = currentUser;
                message = currentUser.FullName;
            }
            var selectedCartIds = HttpContext.Session.GetObject<List<int>>("SelectedCartIds") ?? new List<int>();
            CurrentUsers = currentUser;
            if (currentUser == null)
            {
                return RedirectToPage("/Login/login");
            }
           

            // Fetch order details for the specified order ID
            Orders = await _context.Orders
                .Include(p=>p.User)
                .Include(o=>o.OrderDetails)
                .Where(a => a.UserId.Equals(currentUser.UserId))
                .ToListAsync();

            Orderdetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Include(o => o.Order)
                .Where(od => od.OrderId == id)
                .ToListAsync();

            return Page();
        }
    }
}
