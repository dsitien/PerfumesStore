using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfumesStore.Models;
using System.Text.RegularExpressions;

namespace PerfumesStore.Pages.Login
{
    public class loginModel : PageModel
    {

        private readonly PerfumesShopContext _context;
        private readonly IConfiguration configuration;


        public loginModel(PerfumesShopContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        [BindProperty]
        public string User { get; set; }

        [BindProperty]
        public string Pass { get; set; }

        [BindProperty]
        public bool RememberMe { get; set; }

        public string message { get; set; }
        public User CurrentUsers { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {

            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Pass))
            {
                message = "Please fill in all fields.";
                return Page();
            }

            if (!ValidateUser(User))
            {
                message = "Username must contain 3-20 alphanumeric characters.";
                return Page();
            }

            if (!ValidatePassword(Pass))
            {
                message = "Password must be at least 8 characters long and contain at least one special character.";
                return Page();
            }

            var account = _context.Users.FirstOrDefault(ac => ac.Username.Equals(User) && ac.Password.Equals(Pass));

            if (account == null)
            {

                message = "Invalid login attempt.";
                return Page();
            }

            if (account != null)
            {
                HttpContext.Session.SetObject("currentUser", account);
                if (account.Role.Equals("Admin"))
                {
                    TempData.Remove("message");
                    return RedirectToPage("/Admin/DashBoard/dashboard");
                }

                TempData.Remove("message"); 
                return RedirectToPage("/Index");
            }

            return RedirectToPage("");
        }

        private bool ValidateUser(string username)
        {
            var regex = new Regex("^[a-zA-Z0-9]{3,20}$");
            return regex.IsMatch(username);
        }

        private bool ValidatePassword(string password)
        {
            var regex = new Regex("^(?=.*[a-zA-Z0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{8,}$");
            return regex.IsMatch(password);
        }
    }
}
