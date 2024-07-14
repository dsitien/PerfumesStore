using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.AccountManager
{
    public class DeleteModel : PageModel
    {
        private readonly PerfumesStore.Models.PerfumesShopContext _context;

        public DeleteModel(PerfumesStore.Models.PerfumesShopContext context)
        {
            _context = context;
        }
        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Status = "InActive"; // Change the status instead of deleting
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
