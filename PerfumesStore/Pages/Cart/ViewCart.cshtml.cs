using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages.Cart
{
    public class ViewCartModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly PerfumesShopContext _context;

        public ViewCartModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }
        [BindProperty]
        public IList<Models.Cart> Carts { get; set; } = new List<Models.Cart>();
        public string message { get; set; }
        public string messageError { get; set; }
        public User CurrentUsers { get; set; }
        public IList<Product> ProductList { get; set; }
        public IList<Category> Category { get; set; }
        public IList<Product> filteredProducts { get; set; }
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

            Carts = await _context.Carts.Include(c => c.Product).Where(c => c.UserId.Equals(id)).ToListAsync();

            if (!Carts.Any())
            {
                messageError = "Please select some products to proceed.";
                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<int> selectedCartIds)
        {
            HttpContext.Session.SetObject("SelectedCartIds", selectedCartIds);
            if (!selectedCartIds.Any())
            {
                var currentUser = HttpContext.Session.GetObject<User>("currentUser");
                if (currentUser != null)
                {
                    CurrentUsers = currentUser;
                    message = currentUser.FullName;
                }
                Carts = await _context.Carts.Include(c => c.Product).Where(c => c.UserId == currentUser.UserId && selectedCartIds.Contains(c.CartId)).ToListAsync();
                messageError = "Please select products to proceed.";
                Carts = await _context.Carts.Include(c => c.Product).Where(c => c.UserId.Equals(currentUser.UserId)).ToListAsync();

                if (!Carts.Any())
                {
                    messageError = "Your cart is empty.";
                    return Page();
                }
                return Page();
            }

            return RedirectToPage("/Checkout/Checkout");
        }






        //public async Task<IActionResult> OnPostRemoveFromCartAsync(int productId)
        //{
        //    var currentUser = HttpContext.Session.GetObject<User>("currentUser");
        //    if (currentUser == null)
        //    {
        //        return new JsonResult(new { success = false, message = "User not logged in." });
        //    }

        //    bool result = await _cartService.RemoveFromCartAsync(productId, currentUser.UserId);
        //    if (!result)
        //    {
        //        return new JsonResult(new { success = false, message = "Failed to remove product from cart." });
        //    }

        //    return new JsonResult(new { success = true, productId = productId });
        //}


        public async Task<IActionResult> OnGetRemoveFromCartAsync(int productId)
        {
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            if (currentUser == null)
            {
                // Handle the case where the user is not logged in
                return RedirectToPage("/Account/Login"); // or any page you want to redirect to
            }

            bool result = await _cartService.RemoveFromCartAsync(productId, currentUser.UserId);
            if (!result)
            {
                // Optionally set an error message here
                TempData["ErrorMessage"] = "Failed to remove product from cart.";
                return RedirectToPage("/Cart/ViewCart"); // Redirect to the cart page with error message
            }

            // Optionally set a success message here
            TempData["SuccessMessage"] = "Product removed from cart.";
            return RedirectToPage("/Cart/ViewCart"); // Redirect to the cart page
        }


    }
}
