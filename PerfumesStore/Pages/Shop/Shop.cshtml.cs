using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages.Shop
{
    public class ShopModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PerfumesShopContext _context;
        private readonly Service.CartService _cartService;
        public string message { get; set; }
        public User CurrentUsers { get; set; }
        public IList<Product> ProductList { get; set; }
        public IList<Category> Category { get; set; }
        public IList<Product> filteredProducts { get; set; }
        public IList<Brand> Brands { get; set; }
        public IList<PriceRangeViewModel> PriceRanges { get; set; }
        public ShopModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }


        public async Task OnGetAsync()
        {
            try
            {
                Category = await _context.Categories.ToListAsync();
                Brands = await _context.Brands.ToListAsync();
                PriceRanges = GetPriceRanges();
                var currentUser = HttpContext.Session.GetObject<User>("currentUser");
                if (currentUser != null)
                {
                    CurrentUsers = currentUser;
                    message = currentUser.FullName;
                }
                ProductList = await _context.Products
                                            .Include(p => p.Brand)
                                            .Include(p => p.Category)
                                            .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the products.");
            }

        }


        public async Task<IActionResult> OnGetFilterProductsAsync(string category)
        {
            if (!string.IsNullOrEmpty(category))
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
                    if (checkCategory != null)
                    {
                        filteredProducts = await _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.Brand)
                            .Where(p => p.CategoryId == checkCategory.CategoryId)
                            .ToListAsync();
                    }
                    else
                    {
                        filteredProducts = new List<Product>();
                    }
                }

                return Partial("_ProductListPartial", filteredProducts);
            }

            return BadRequest();
        }


        public async Task<IActionResult> OnGetFilterBrandProductsAsync(string brand)
        {
            if (!string.IsNullOrEmpty(brand))
            {
                if (brand.Equals("all"))
                {
                    filteredProducts = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .ToListAsync();
                }
                else
                {
                    var checkBrand = await _context.Brands.FirstOrDefaultAsync(c => c.BrandName.Equals(brand));
                    if (checkBrand != null)
                    {
                        filteredProducts = await _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.Brand)
                            .Where(p => p.BrandId == checkBrand.BrandId)
                            .ToListAsync();
                    }
                    else
                    {
                        filteredProducts = new List<Product>();
                    }
                }

                return Partial("_ProductListPartial", filteredProducts);
            }

            return BadRequest();
        }


        public async Task<IActionResult> OnGetFilterPriceProductsAsync(string price)
        {
            if (!string.IsNullOrEmpty(price))
            {
                if (price.Equals("all"))
                {
                    filteredProducts = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .ToListAsync();
                }
                else
                {
                    // Parse the price range from the string input
                    var priceRange = ParsePriceRange(price);
                    if (priceRange != null)
                    {
                        filteredProducts = await _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.Brand)
                            .Where(p => p.Price >= priceRange.Value.Item1 && p.Price <= priceRange.Value.Item2)
                            .ToListAsync();
                    }
                    else
                    {
                        filteredProducts = new List<Product>();
                    }
                }

                return Partial("_ProductListPartial", filteredProducts);
            }

            return BadRequest();
        }



        private List<PriceRangeViewModel> GetPriceRanges()
        {
            return new List<PriceRangeViewModel>
            {
                new PriceRangeViewModel { Value = "0-100000", Text = "Under 100.000đ" },
                new PriceRangeViewModel { Value = "100000-200000", Text = "100.000đ - 200.000đ" },
                new PriceRangeViewModel { Value = "200000-300000", Text = "200.000đ - 300.000đ" },
                new PriceRangeViewModel { Value = "300000-500000", Text = "300.000đ - 500.000đ" },
                new PriceRangeViewModel { Value = "500000-1000000", Text = "500.000đ - 1.000.000đ" },
                new PriceRangeViewModel { Value = "1000000", Text = "Over 1.000.000đ" }
            };
        }

        private (decimal, decimal)? ParsePriceRange(string price)
        {
            switch (price)
            {
                case "Under 100.000đ":
                    return (0, 100000);
                case "100.000đ - 200.000đ":
                    return (100000, 200000);
                case "200.000đ - 300.000đ":
                    return (200000, 300000);
                case "300.000đ - 500.000đ":
                    return (300000, 500000);
                case "500.000đ - 1.000.000đ":
                    return (500000, 1000000);
                case "Over 1.000.000đ":
                    return (1000000, 10000000);
                default:
                    return null;
            }
        }

    }
    public class PriceRangeViewModel
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }




}
