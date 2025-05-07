using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;


namespace MaharlikaGrandEstate.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; } = string.Empty;

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Ensure Email is not null (compiler-safe)
                if (string.IsNullOrEmpty(Input.Email))
                {
                    ModelState.AddModelError(string.Empty, "Email is required.");
                    return Page();
                }

                // Safe to use Input.Email now
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                        returnUrl = "~/Admin/Dashboard";
                    else if (roles.Contains("Lister"))
                        returnUrl = "~/Lister/Dashboard";
                    else
                        returnUrl = "~/Buyer/Browse";
                }

                // Enable lockout on failure by setting lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogError("Login failed for user: {Email}", Input.Email);
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been locked out. Please try again in 5 minutes.");
                    return Page();
                }
                else
                {
                    if (user != null)
                    {
                        // Check the current number of failed login attempts
                        var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                        if (failedCount >= 4)
                        {
                            ModelState.AddModelError(string.Empty, "Warning: One more failed attempt will lock your account for 5 minutes.");
                        }
                    }
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }
    }


}
