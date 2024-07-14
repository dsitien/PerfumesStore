using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Admin.BrandManager
{
    public class DeleteModel : PageModel
    {
        private readonly PerfumesStore.Models.PerfumesShopContext _context;

        public DeleteModel(PerfumesStore.Models.PerfumesShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Brand Brand { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FirstOrDefaultAsync(m => m.BrandId == id);

            if (brand == null)
            {
                return NotFound();
            }
            else
            {
                Brand = brand;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                brand.Status = "Inactive"; // Thay đổi trạng thái thành Inactive
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
