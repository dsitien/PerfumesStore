using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfumesStore.Models;
using System.ComponentModel.DataAnnotations;
using PerfumesStore.Service;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace PerfumesStore.Pages.Register
{
    public class registerModel : PageModel
    {
        private readonly PerfumesShopContext _context;
        private readonly Service.IEmaiService _emailService;
        public registerModel(PerfumesShopContext context, IEmaiService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        [TempData]
        public string message { get; set; }
        public User CurrentUsers { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingUser = _context.Users.FirstOrDefault(ac => ac.Username.Equals(Input.UserName));
            if (existingUser != null)
            {
                ModelState.AddModelError("Input.UserName", "Username already exists!");
                return Page();
            }

            var existingEmail = _context.Users.FirstOrDefault(ac => ac.Email.Equals(Input.Email));
            if (existingEmail != null)
            {
                ModelState.AddModelError("Input.Email", "Email already exists!");
                return Page();
            }

            var existingPhone = _context.Users.FirstOrDefault(ac => ac.Phone.Equals(Input.PhoneNumber));
            if (existingPhone != null)
            {
                ModelState.AddModelError("Input.PhoneNumber", "Phone number already exists!");
                return Page();
            }



            var verificationCode = GenerateRandomCode();
            HttpContext.Session.SetString("VerificationCode", verificationCode);
            HttpContext.Session.SetString("RegisterInputModel", System.Text.Json.JsonSerializer.Serialize(Input));

            var user = new User
            {
                Username = Input.UserName,
                Password = Input.Password, // Ideally, you should hash the password
                Email = Input.Email,
                FullName = Input.FullName,
                Phone = Input.PhoneNumber,
                Address = Input.Address,
                Status = "InActive", // Set default status
                Role = "User" // Set default role
            };

            _context.Users.Add(user);

            if (ModelState.IsValid)
            {
                string ToEmail = Input.Email;
                string Subject = "Register Verification";

                
                string Body = GenerateEmailBody(verificationCode);
                await _emailService.SendEmailAsync(ToEmail, Subject, Body);
                return RedirectToPage("/Register/RegisterVerify");
                
            }
            return Page();
        }



        private string GenerateRandomCode()
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                var random = BitConverter.ToUInt32(bytes, 0);
                var password = new StringBuilder();
                while (password.Length < 6)
                {
                    password.Append(validChars[(int)(random % (uint)validChars.Length)]);
                    random /= (uint)validChars.Length;
                }
                return password.ToString();
            }
        }


        private string GenerateEmailBody(string generatedPassword)
        {
            string emailBody = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Welcome to Our Platform</title>
                    <style>
                        /* CSS styles for better email rendering */
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            background-color: #f4f4f4;
                            padding: 20px;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            background: #fff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        h2 {{
                            color: #333;
                        }}
                        p {{
                            color: #666;
                        }}
                        .cta-button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #007bff;
                            color: #fff;
                            text-decoration: none;
                            border-radius: 5px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Welcome to Our Perfumes Store!</h2>
                        <p>Thank you for registering an account with us. To complete your registration, please follow these steps:</p>
                        <ol>
                            <li>Your temporary code: <strong>{generatedPassword}</strong></li>
                            <li>Use this code to verify to your account.</li>
                        </ol>
                        <p>If you have any questions or need further assistance, please feel free to contact us.</p>
                        <p>Thank you,<br>Team Perfumes Store</p>
                    </div>
                </body>
                </html>
            ";

            return emailBody;
        }

    }

    public class RegisterInputModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only letters and numbers are accepted.")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[a-zA-Z0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{8,}$", ErrorMessage = "Password must contain at least 8 characters and 1 special character.")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
