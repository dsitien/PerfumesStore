using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.AccountManager
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

        public IList<User> User { get; set; } = new List<User>();
        public string CurrentSort { get; set; }
        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public PaginatedList<User> Users { get; set; }

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
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
            }

            IQueryable<User> usersIQ = from u in _context.Users select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                usersIQ = usersIQ.Where(u => u.Username.ToUpper().Contains(searchString) || u.Email.ToUpper().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    usersIQ = usersIQ.OrderByDescending(u => u.Username);
                    break;
                case "Date":
                    usersIQ = usersIQ.OrderBy(u => u.Email); // Assuming CreatedAt is used for sorting
                    break;
                case "date_desc":
                    usersIQ = usersIQ.OrderByDescending(u => u.FullName); // Assuming CreatedAt is used for sorting
                    break;
                default:
                    usersIQ = usersIQ.OrderBy(u => u.Username);
                    break;
            }

            var pageSize = _configuration.GetValue("PageSize", 4);
            Users = await PaginatedList<User>.CreateAsync(usersIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
