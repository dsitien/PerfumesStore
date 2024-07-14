using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using PerfumesStore.Service;

namespace PerfumesStore.Pages.Checkout
{
    public class CheckoutModel : PageModel
    {

        private readonly PerfumesShopContext _context;
        private readonly CartService _cartService;


        public CheckoutModel(PerfumesShopContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [BindProperty]
        public User CurrentUser { get; set; }
        [BindProperty]
        public IList<Models.Cart> Carts { get; set; } = new List<Models.Cart>();
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string ShippingAddress { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        public string Message { get; set; }
        public List<int> SelectedCartIds { get; set; }



        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            var selectedCartIds = HttpContext.Session.GetObject<List<int>>("SelectedCartIds") ?? new List<int>();
            SelectedCartIds = selectedCartIds.ToList();
          

            if (currentUser == null)
            {
                return RedirectToPage("/Login/login");
            }
          
            CurrentUser = currentUser;
            Carts = await _context.Carts.Include(c => c.Product).Where(c => c.UserId == currentUser.UserId && selectedCartIds.Contains(c.CartId)).ToListAsync();

            if (!Carts.Any())
            {
                return RedirectToPage("/Cart/ViewCart", new { id = currentUser.UserId });
            }

            return Page();
        }




        public async Task<IActionResult> OnPostAsync()
        {
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            var selectedCartIds = HttpContext.Session.GetObject<List<int>>("SelectedCartIds") ?? new List<int>();
            SelectedCartIds = selectedCartIds.ToList();
            if (currentUser == null)
            {
                return RedirectToPage("/Login/login");
            }

            Carts = await _context.Carts.Include(c => c.Product).Where(c => c.UserId == currentUser.UserId && selectedCartIds.Contains(c.CartId)).ToListAsync();
            CurrentUser = currentUser;
            // Create the order
            decimal totalAmount = Carts.Sum(c => c.Quantity * c.Product.Price);
            var order = new Order
            {
                UserId = currentUser.UserId,
                CreatedAt = DateTime.Now.Date,
                TotalAmount = totalAmount,
                Status = "wait to confirm"
            };

            // Add order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add order items to the database
            foreach (var cartItem in Carts)
            {
                var orderDetails = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    Fullname = FullName,
                    Phone = Phone,
                    ShippingAddress = ShippingAddress
                };

                _context.OrderDetails.Add(orderDetails);

                // Remove the item from the cart
                _context.Carts.Remove(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Orders/OrderDetails", new { id = order.OrderId });
        }

    }
}
