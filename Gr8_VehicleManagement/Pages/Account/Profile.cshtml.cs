using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Gr8_VehicleManagement.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;

        public ProfileModel(IUserService userService, ICustomerService customerService)
        {
            _userService = userService;
            _customerService = customerService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [BindProperty]
        public PasswordInputModel PasswordInput { get; set; } = new PasswordInputModel();

        public string UserType { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Họ tên là bắt buộc")]
            [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
            public string PhoneNumber { get; set; } = string.Empty;

            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [StringLength(256, ErrorMessage = "Email không được vượt quá 256 ký tự")]
            public string? Email { get; set; }

            [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
            public string? Address { get; set; }

            [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
            [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
            public string Username { get; set; } = string.Empty;
        }

        public class PasswordInputModel
        {
            [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
            public string CurrentPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6-100 ký tự")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Xác nhận mật khẩu mới là bắt buộc")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            var userId = HttpContext.Session.GetUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var userResult = await _userService.GetUserByIdAsync(userId!.Value);
                if (!userResult.Success || userResult.Data == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin người dùng.";
                    return RedirectToPage("/Account/Login");
                }

                var user = userResult.Data;
                Input = new InputModel
                {
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Username = user.Username
                };

                UserType = user.UserType.ToString();
                CreatedAt = user.CreatedAt;

                // Lấy thông tin customer nếu có
                if (user.UserType == Data.Enums.UserType.Customer)
                {
                    var customerResult = await _customerService.GetCustomerByUserIdAsync(userId!.Value);
                    if (customerResult.Success && customerResult.Data != null)
                    {
                        Input.Address = customerResult.Data.Address;
                    }
                }

                return Page();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông tin.";
                return RedirectToPage("/Account/Login");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = HttpContext.Session.GetUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var updateUserDto = new UpdateUserDto
                {
                    Id = userId.Value,
                    FullName = Input.FullName,
                    PhoneNumber = Input.PhoneNumber,
                    Email = Input.Email
                };

                var userResult = await _userService.UpdateUserAsync(updateUserDto);
                if (!userResult.Success)
                {
                    ModelState.AddModelError(string.Empty, userResult.Message ?? "Không thể cập nhật thông tin người dùng.");
                    return Page();
                }

                // Cập nhật thông tin customer nếu cần
                var userType = HttpContext.Session.GetUserType();
                if (userType.HasValue && userType.Value == Data.Enums.UserType.Customer)
                {
                    var customerResult = await _customerService.GetCustomerByUserIdAsync(userId!.Value);
                    if (customerResult.Success && customerResult.Data != null)
                    {
                        var updateCustomerDto = new UpdateCustomerDto
                        {
                            Id = customerResult.Data.Id,
                            FullName = Input.FullName,
                            PhoneNumber = Input.PhoneNumber,
                            Email = Input.Email,
                            Address = Input.Address,
                            CustomerType = customerResult.Data.CustomerType
                        };

                        await _customerService.UpdateCustomerAsync(updateCustomerDto, userId.Value);
                    }
                }

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToPage();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật thông tin.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            if (!HttpContext.Session.IsAuthenticated())
            {
                return RedirectToPage("/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = HttpContext.Session.GetUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var changePasswordDto = new ChangePasswordDto
                {
                    UserId = userId.Value,
                    CurrentPassword = PasswordInput.CurrentPassword,
                    NewPassword = PasswordInput.NewPassword
                };

                var result = await _userService.ChangePasswordAsync(changePasswordDto);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Không thể đổi mật khẩu.");
                    return Page();
                }

                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToPage();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đổi mật khẩu.");
                return Page();
            }
        }
    }
}
