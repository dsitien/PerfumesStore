using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages.Logout
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PerfumesShopContext _context;
        private readonly Service.CartService _cartService;
        public string message { get; set; }
        public User CurrentUsers { get; set; }


        public LogoutModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IList<Product> ProductList { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {

            try
            {
                HttpContext.Session.Clear();
                ProductList = await _context.Products
                                            .Include(p => p.Brand)
                                            .Include(p => p.Category)
                                            .ToListAsync();
                return RedirectToPage("/Index");
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the products.");
                return Page();
            }
        }
    }
}
