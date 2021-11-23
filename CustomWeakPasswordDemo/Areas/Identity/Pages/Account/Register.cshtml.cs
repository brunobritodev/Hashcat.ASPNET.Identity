using CustomWeakPasswordDemo.Data;
using CustomWeakPasswordDemo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomWeakPasswordDemo.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly ICustomUserService _customUserService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            ICustomUserService customUserService,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration,
            ILogger<RegisterModel> logger)
        {
            _customUserService = customUserService;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }

        public void OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User { Username = Input.Email, HashType = _configuration["PasswordType"] };
                var result = await _customUserService.RegisterUser(user, Input.Password);
                if (!result.Any())
                {
                    _logger.LogInformation("User created a new account with password.");
                    var userDb = await _customUserService.Login(user.Username, Input.Password);
                    var claims = new List<Claim>()
                    {
                        new Claim("username", userDb.Username)
                    };

                    var claimsIdentity = new List<ClaimsIdentity>() { new ClaimsIdentity(claims, "pwd") };
                    await _contextAccessor.HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
                    _logger.LogInformation("User logged in.");

                    var callbackUrl = Url.Page(
                        "/Account/Login",
                        pageHandler: null,
                        values: null,
                        protocol: Request.Scheme);

                }
                foreach (var error in result)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
