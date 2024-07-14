using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;
using System.Threading.Tasks;

namespace PerfumesStore.Pages.Profile
{
    public class EditProfileModel : PageModel
    {
        private readonly PerfumesShopContext _context;

        public EditProfileModel(PerfumesShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public User CurrentUsers { get; set; }
        public string message { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            if (currentUser != null)
            {
                CurrentUsers = currentUser;
                message = currentUser.FullName;
            }
            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Ensure User.UserId is populated correctly in your Razor Page form
            User.UserId = id;

            if (ModelState.IsValid)
            {
                return Page(); // Return the page with validation errors
            }

            try
            {
                // Retrieve the existing user from the database
                var userToUpdate = await _context.Users.FindAsync(User.UserId);

                if (userToUpdate == null)
                {
                    return NotFound();
                }

                // Update only the specified fields
                userToUpdate.FullName = User.FullName;
                userToUpdate.Phone = User.Phone;
                userToUpdate.Address = User.Address;

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Redirect to view profile page after successful update
            return RedirectToPage("/Profile/ViewProfile", new { id = User.UserId });
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
