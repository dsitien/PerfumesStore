using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.BrandManager
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
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Brand> Brands { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

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

            IQueryable<Brand> brandsIQ = from b in _context.Brands select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                brandsIQ = brandsIQ.Where(b => b.BrandName.Contains(searchString) || b.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    brandsIQ = brandsIQ.OrderByDescending(b => b.BrandName);
                    break;
                case "Date":
                    brandsIQ = brandsIQ.OrderBy(b => b.BrandId); // Assuming BrandId is used for sorting
                    break;
                case "date_desc":
                    brandsIQ = brandsIQ.OrderByDescending(b => b.BrandId); // Assuming BrandId is used for sorting
                    break;
                default:
                    brandsIQ = brandsIQ.OrderBy(b => b.BrandName);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 4);
            Brands = await PaginatedList<Brand>.CreateAsync(brandsIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
