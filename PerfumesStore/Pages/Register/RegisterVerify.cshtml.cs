using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using PerfumesStore.Service;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Register
{

    public class RegisterVerifyModel : PageModel
    {
        private readonly PerfumesShopContext _context;
        public RegisterVerifyModel(PerfumesShopContext context)
        {
            _context = context;
        }



        [BindProperty]
        public string VerificationCode { get; set; }
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Please fill in all fields.";
                    return Page();
            }

            // Find the user with the matching verification code
            var storedCode = HttpContext.Session.GetString("VerificationCode");

            if (storedCode != VerificationCode)
            {

                Message = "Invalid verification code.";
                return Page();
            }

            var userInputJson = HttpContext.Session.GetString("RegisterInputModel");
            var userInput = System.Text.Json.JsonSerializer.Deserialize<RegisterInputModel>(userInputJson);

            var user = new User
            {
                Username = userInput.UserName,
                Password = userInput.Password, // Ideally, you should hash the password
                Email = userInput.Email,
                FullName = userInput.FullName,
                Phone = userInput.PhoneNumber,
                Address = userInput.Address,
                Status = "Active", // Set status to active upon verification
                Role = "User" // Set default role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("VerificationCode");
            HttpContext.Session.Remove("RegisterInputModel");

            TempData["Message"] = "Email verified successfully!";
            return RedirectToPage("/Login/login");
        }



    }
}
