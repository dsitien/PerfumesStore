using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.ProductManager
{
    public class CreateModel : PageModel
    {
        private readonly PerfumesShopContext _context;

        public CreateModel(PerfumesShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile ProductImg { get; set; }

        public IActionResult OnGet()
        {
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

            // Check for duplicate product name
            bool productExists = _context.Products.Any(p => p.ProductName == Product.ProductName);
            if (productExists)
            {
                ModelState.AddModelError("Product.ProductName", "A product with the same name already exists.");
                var activeBrands = _context.Brands.Where(b => b.Status == "Active").ToList();
                var activeCategories = _context.Categories.Where(c => c.Status == "Active").ToList();

                ViewData["Brands"] = new SelectList(activeBrands, "BrandId", "BrandName");
                ViewData["Categories"] = new SelectList(activeCategories, "CategoryId", "CategoryName");
                return Page();
            }

            if (ProductImg != null && ProductImg.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ProductImg.CopyToAsync(memoryStream);
                    Product.ProductImg = memoryStream.ToArray();
                }
            }
            Product.Status = "Active";
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
