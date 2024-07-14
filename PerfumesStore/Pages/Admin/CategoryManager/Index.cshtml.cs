using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.CategoryManager
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

        public PaginatedList<Category> Categories { get; set; }

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

            IQueryable<Category> categoriesIQ = from c in _context.Categories select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                categoriesIQ = categoriesIQ.Where(c => c.CategoryName.Contains(searchString) || c.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    categoriesIQ = categoriesIQ.OrderByDescending(c => c.CategoryName);
                    break;
                case "Date":
                    categoriesIQ = categoriesIQ.OrderBy(c => c.CategoryId); // Assuming CategoryId is the date-related field
                    break;
                case "date_desc":
                    categoriesIQ = categoriesIQ.OrderByDescending(c => c.CategoryId); // Assuming CategoryId is the date-related field
                    break;
                default:
                    categoriesIQ = categoriesIQ.OrderBy(c => c.CategoryName);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 4);
            Categories = await PaginatedList<Category>.CreateAsync(categoriesIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
