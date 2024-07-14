using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.ProductManager
{
    public class EditModel : PageModel
    {
        private readonly PerfumesShopContext _context;

        public EditModel(PerfumesShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile ProductImg { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products.FindAsync(id);

            if (Product == null)
            {
                return NotFound();
            }

            var activeBrands = _context.Brands.Where(b => b.Status == "Active").ToList();
            var activeCategories = _context.Categories.Where(c => c.Status == "Active").ToList();

            ViewData["Brands"] = new SelectList(activeBrands, "BrandId", "BrandName");
            ViewData["Categories"] = new SelectList(activeCategories, "CategoryId", "CategoryName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var activeBrands = _context.Brands.Where(b => b.Status == "Active").ToList();
                var activeCategories = _context.Categories.Where(c => c.Status == "Active").ToList();

                ViewData["Brands"] = new SelectList(activeBrands, "BrandId", "BrandName");
                ViewData["Categories"] = new SelectList(activeCategories, "CategoryId", "CategoryName");

                return Page();
            }

            var productToUpdate = await _context.Products.FindAsync(Product.ProductId);
            if (productToUpdate == null)
            {
                return NotFound();
            }

            if (ProductImg != null && ProductImg.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ProductImg.CopyToAsync(memoryStream);
                    productToUpdate.ProductImg = memoryStream.ToArray();
                }
            }

            productToUpdate.ProductName = Product.ProductName;
            productToUpdate.Price = Product.Price;
            productToUpdate.Description = Product.Description;
            productToUpdate.Stock = Product.Stock;
            productToUpdate.Status = Product.Status;
            productToUpdate.BrandId = Product.BrandId;
            productToUpdate.CategoryId = Product.CategoryId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
