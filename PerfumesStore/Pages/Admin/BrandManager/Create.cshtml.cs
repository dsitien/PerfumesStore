using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.BrandManager
{
    public class CreateModel : PageModel
    {
        private readonly PerfumesStore.Models.PerfumesShopContext _context;

        public CreateModel(PerfumesStore.Models.PerfumesShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Brand Brand { get; set; } = new Brand();

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
           /* // Check for duplicate BrandId
            if (await _context.Brands.AnyAsync(b => b.BrandId == Brand.BrandId))
            {
                ModelState.AddModelError("Brand.BrandId", "A brand with this ID already exists.");
                return Page();
            }*/
            Brand.Status = "Active"; // Set the default status to "Active"
            _context.Brands.Add(Brand);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
