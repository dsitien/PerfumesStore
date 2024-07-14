using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PerfumesShopContext _context;
        private readonly Service.CartService _cartService;
        public string message { get; set; }
        public User CurrentUsers { get; set; }


        public IndexModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IList<Product> ProductList { get; set; }
        public IList<Category> Category { get; set; }
        public IList<Product> bestSellerProducts { get; set; }
        public IList<Product> filteredProducts { get; set; }


        public async Task OnGetAsync()
        {
            try
            {
                var currentUser = HttpContext.Session.GetObject<User>("currentUser");
                if (currentUser != null)
                {
                    CurrentUsers = currentUser;
                    message = currentUser.FullName;
                }

                Category = await _context.Categories.ToListAsync();

                ProductList = await _context.Products
                                            .Include(p => p.Brand)
                                            .Include(p => p.Category)
                                            .ToListAsync();

                var bestSellers = await _context.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantitySold = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(g => g.TotalQuantitySold)
                    .Take(5) // Take top 5 best-selling products
                    .ToListAsync();
                bestSellerProducts = new List<Product>();
                foreach (var item in bestSellers)
                {
                    var product = await _context.Products
                        .Include(p => p.Brand)
                        .Include(p => p.Category)
                        .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                    if (product != null)
                    {
                        bestSellerProducts.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the products.");
            }
        }


        public async Task<IActionResult> OnGetFilterProductsAsync(string category)
        {
            if (!category.IsNullOrEmpty())
            {
                
                if (category.Equals("all"))
                {
                    Category = await _context.Categories.ToListAsync();
                    filteredProducts = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .ToListAsync();
                }
                else
                {
                    Category = await _context.Categories.ToListAsync();
                    var checkCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName.Equals(category));

                    filteredProducts = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Brand)
                    .Where(p => p.CategoryId == checkCategory.CategoryId)
                    .ToListAsync();

                }

                return Partial("_ProductListPartial", filteredProducts);
            }

            // Handle invalid category or return default content
            return BadRequest();
        }

    }
}
