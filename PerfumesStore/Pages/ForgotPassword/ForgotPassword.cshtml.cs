using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfumesStore.Models;
using PerfumesStore.Service;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PerfumesStore.Pages.ForgotPassword
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly PerfumesShopContext _context;
        private readonly Service.IEmaiService _emailService;
        public ForgotPasswordModel(PerfumesShopContext context, IEmaiService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [BindProperty]
        public string User { get; set; }

        [BindProperty]
        public string Email { get; set; }
        public string Message { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Email))
            {
                Message = "Please fill in all fields.";
                return Page();
            }

            if (!ValidateUser(User))
            {
                Message = "Username must contain 3-20 alphanumeric characters.";
                return Page();
            }

            var account = _context.Users.FirstOrDefault(ac => ac.Username.Equals(User) && ac.Email.Equals(Email));

            if (account == null)
            {
                Message = "Invalid Email or Username. Please check your credentials and try again !";
                return Page();
            }
            if (account != null)
            {
                if (ModelState.IsValid)
                {
                    var verificationCode = GenerateRandomCode();
                    
                    string ToEmail = Email;
                    string Subject = "Reset Password";
                    string Body = GenerateEmailBody(verificationCode);

                    account.Password = verificationCode;
                    await _context.SaveChangesAsync();

                    await _emailService.SendEmailAsync(ToEmail, Subject, Body);
                    return RedirectToPage("/Login/login");

                }
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
                while (password.Length < 8)
                {
                    password.Append(validChars[(int)(random % (uint)validChars.Length)]);
                    random /= (uint)validChars.Length;
                }
                
                return password.ToString()+"@";
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
                        <p>We have received a request to reset the password for your account on Perfumes Store:</p>
                        <ol>
                            <li>Your temporary Password: <strong>{generatedPassword}</strong></li>
                            <li>Use this code to Login to your account.</li>
                        </ol>
                        <p>If you have any questions or need further assistance, please feel free to contact us.</p>
                        <p>Thank you,<br>Team Perfumes Store</p>
                    </div>
                </body>
                </html>
            ";

            return emailBody;
        }

        public void OnGet()
        {
        }

        private bool ValidateUser(string username)
        {
            var regex = new Regex("^[a-zA-Z0-9]{3,20}$");
            return regex.IsMatch(username);
        }
    }
}
