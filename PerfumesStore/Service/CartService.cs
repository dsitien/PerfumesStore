using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Service
{
    public class CartService
    {

        private readonly PerfumesShopContext _context;

        public CartService(PerfumesShopContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToCartAsync(int productId, int userId, int quantity)
        {
            try
            {
                // Retrieve the product from the database
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                {
                    return false; // Product not found
                }

                var checkCart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
                if (checkCart == null)
                {
                    // Create a new Cart item
                    var cartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = productId,
                        Price = product.Price,
                        Quantity = quantity // Default quantity, adjust as needed
                    };
                    // Add to database
                    _context.Carts.Add(cartItem);
                    await _context.SaveChangesAsync();

                }
                else
                {
                    checkCart.Quantity += quantity;
                    await _context.SaveChangesAsync();
                }

                return true; // Successfully added to cart
            }
            catch (Exception)
            {
                // Handle exceptions here if needed
                return false; // Failed to add to cart
            }
        }


        public async Task<bool> RemoveFromCartAsync(int productId, int userId)
        {
            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == productId && c.UserId == userId);
            if (cartItem == null)
            {
                return false;
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
