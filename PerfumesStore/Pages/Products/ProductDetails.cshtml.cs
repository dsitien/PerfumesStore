using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages.Products
{
    public class ProductDetailsModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly PerfumesShopContext _context;

        public ProductDetailsModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        public string message { get; set; }
        public User CurrentUsers { get; set; }

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

            Product = await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(a => a.ProductId.Equals(id));
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }

        [BindProperty]
        public int Quantity { get; set; }
      

        [BindProperty]
        public int ProductId { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
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

            var checkStockInCart = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == ProductId && c.UserId.Equals(currentUser.UserId));
            if (checkStockInCart != null)
            {
                var checkProduct_StockInCart = await _context.Products.FirstOrDefaultAsync(p => p.ProductId.Equals(ProductId));
                if (checkProduct_StockInCart == null)
                {
                    TempData["FailMessage"] = "Product not found.";
                    return RedirectToPage("/Products/ProductDetails", new { id = ProductId });
                }
                if (checkProduct_StockInCart.Stock <= checkStockInCart.Quantity)
                {
                    TempData["FailMessage"] = "The Quantity in cart exceeds the available stock of product.";
                    return RedirectToPage("/Products/ProductDetails", new { id = ProductId });
                }
            }
            var checkProduct_Stock = await _context.Products.FirstOrDefaultAsync(p => p.ProductId.Equals(ProductId));
            if (checkProduct_Stock == null)
            {
                TempData["FailMessage"] = "Product not found.";
                return RedirectToPage("/Products/ProductDetails", new { id = ProductId });
            }
            if (checkProduct_Stock.Stock < Quantity)
            {
                TempData["FailMessage"] = "The Quantity in cart exceeds the available stock of product.";
                return RedirectToPage("/Products/ProductDetails", new { id = ProductId });
            }

            bool result = await _cartService.AddToCartAsync(ProductId, currentUser.UserId, Quantity);
            if (!result)
            {
                // Handle failure to add product to cart (e.g., show error message)
                TempData["Message"] = "Failed to add product to cart.";
            }
            else
            {
                // Successful addition to cart
                TempData["Message"] = "Product added to cart successfully.";
            }

            return RedirectToPage("/Products/ProductDetails", new { id = ProductId });
        }
    }
}
