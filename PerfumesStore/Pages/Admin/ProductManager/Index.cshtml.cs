using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.ProductManager
{
    public class IndexModel : PageModel
    {
        private readonly PerfumesShopContext _context;
        private readonly IConfiguration _configuration;

        public IndexModel(PerfumesShopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string NameSort { get; set; }
        public string PriceSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Product> Products { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            // Convert searchString to uppercase
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
            }

            IQueryable<Product> productsIQ = from p in _context.Products
                                             .Include(p => p.Brand)
                                             .Include(p => p.Category)
                                             select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                productsIQ = productsIQ.Where(p => p.ProductName.ToUpper().Contains(searchString)
                                                 || p.Category.CategoryName.ToUpper().Contains(searchString)
                                                 || p.Brand.BrandName.ToUpper().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    productsIQ = productsIQ.OrderByDescending(p => p.ProductName);
                    break;
                case "Price":
                    productsIQ = productsIQ.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productsIQ = productsIQ.OrderByDescending(p => p.Price);
                    break;
                default:
                    productsIQ = productsIQ.OrderBy(p => p.ProductName);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 4);
            Products = await PaginatedList<Product>.CreateAsync(productsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        public IActionResult GetImage(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product != null && product.ProductImg != null)
            {
                return File(product.ProductImg, "image/jpg");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
