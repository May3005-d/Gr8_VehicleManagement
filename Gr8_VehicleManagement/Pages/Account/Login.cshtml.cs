using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IAuthService authService, ILogger<LoginModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [BindProperty]
        public LoginDto Input { get; set; } = new LoginDto();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            // If already logged in, redirect to dashboard
            if (HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Dashboard/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _authService.LoginAsync(Input);

                if (!result.Success)
                {
                    ErrorMessage = result.Message;
                    _logger.LogWarning("Login failed for {Username}: {Message}", Input.Username, result.Message);
                    return Page();
                }

                // Set session
                HttpContext.Session.SetUserSession(result.Data!);

                _logger.LogInformation("User {UserId} logged in successfully", result.Data!.Id);

                // Redirect based on return URL or user type
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                // Redirect to appropriate dashboard
                return result.Data.UserType switch
                {
                    Data.Enums.UserType.Admin => RedirectToPage("/Dashboard/Admin"),
                    Data.Enums.UserType.EVMStaff => RedirectToPage("/Dashboard/EVMStaff"),
                    Data.Enums.UserType.DealerManager => RedirectToPage("/Dashboard/DealerManager"),
                    Data.Enums.UserType.DealerStaff => RedirectToPage("/Dashboard/DealerStaff"),
                    Data.Enums.UserType.Customer => RedirectToPage("/Dashboard/Customer"),
                    _ => RedirectToPage("/Index")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                ErrorMessage = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.";
                return Page();
            }
        }
    }
}

