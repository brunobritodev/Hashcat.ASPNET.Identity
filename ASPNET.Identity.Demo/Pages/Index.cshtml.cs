using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNET.Identity.Demo.Data;
using Microsoft.AspNetCore.Identity;
using NetDevPack.Utilities;

namespace ASPNET.Identity.Demo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public void OnGet()
        {
            if (User.Identity.IsAuthenticated)
                this.Users = _context.Users.Select(s => new CustomUser(s)).ToList();
        }

        [BindProperty]
        public List<CustomUser> Users { get; set; }
    }

    public class CustomUser
    {
        public CustomUser(IdentityUser identityUser)
        {
            User = identityUser;
            HashInfo = new AspNetIdentityHashInfo(identityUser.PasswordHash);
        }

        public AspNetIdentityHashInfo HashInfo { get; set; }

        public IdentityUser User { get; set; }
    }
}
