using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using System.Threading.Tasks;

namespace PerfumesStore.Pages.Profile
{
    public class ViewProfileModel : PageModel
    {
        private readonly PerfumesShopContext _context;
        [BindProperty]
        public User CurrentUsers { get; set; }
        public string message { get; set; }
        public ViewProfileModel(PerfumesShopContext context)
        {
            _context = context;
        }

        public User User { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            if (currentUser != null)
            {
                CurrentUsers = currentUser;
                message = currentUser.FullName;
            }

            User = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
