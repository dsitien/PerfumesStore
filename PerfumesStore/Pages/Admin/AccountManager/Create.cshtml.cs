using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.AccountManager
{
    public class CreateModel : PageModel
    {
        private readonly PerfumesShopContext _context;

        public CreateModel(PerfumesShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                return Page();
            }

            // Check for duplicate username
            if (await _context.Users.AnyAsync(u => u.Username == User.Username))
            {
                ModelState.AddModelError("User.Username", "Username is already taken.");
                return Page();
            }

            // Check for duplicate email
            if (await _context.Users.AnyAsync(u => u.Email == User.Email))
            {
                ModelState.AddModelError("User.Email", "Email is already in use.");
                return Page();
            }

            // Check for duplicate phone number
            if (await _context.Users.AnyAsync(u => u.Phone == User.Phone))
            {
                ModelState.AddModelError("User.Phone", "Phone number is already in use.");
                return Page();
            }

            // Hash the password before saving to the database
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);
            User.Status = "Active"; // Set default status

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/AccountManager/Index");
        }
    }
}
