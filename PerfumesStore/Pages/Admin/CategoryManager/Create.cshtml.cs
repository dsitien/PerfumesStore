using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.CategoryManager
{
    public class CreateModel : PageModel
    {
        private readonly PerfumesStore.Models.PerfumesShopContext _context;

        public CreateModel(PerfumesStore.Models.PerfumesShopContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                return Page();
            }

            // Check if CategoryId already exists
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == Category.CategoryId);
           /* if (existingCategory != null)
            {
                ModelState.AddModelError(nameof(Category.CategoryId), "Category ID already exists. Please choose a different ID.");
                return Page();
            }*/
            Category.Status = "Active";
            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
