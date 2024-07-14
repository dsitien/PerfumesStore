using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PerfumesStore.Models;

namespace PerfumesStore.Pages.Chat
{
    public class ChatModel : PageModel
    {

        private readonly PerfumesShopContext _context;
        private readonly IConfiguration configuration;


        public ChatModel(PerfumesShopContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }
        public string message { get; set; }
        public User CurrentUsers { get; set; }

        public void OnGet()
        {
            var currentUser = HttpContext.Session.GetObject<User>("currentUser");
            if (currentUser != null)
            {
                CurrentUsers = currentUser;
                message = currentUser.FullName;
            }
        }
    }
}
