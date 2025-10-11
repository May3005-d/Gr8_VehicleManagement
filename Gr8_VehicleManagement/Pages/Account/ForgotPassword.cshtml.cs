using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Gr8_VehicleManagement.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IUserService _userService;

        public ForgotPasswordModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
            [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải từ 6-100 ký tự")]
            public string NewPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Xác nhận mật khẩu mới là bắt buộc")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
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
                // Tìm user theo username
                var userResult = await _userService.GetUserByUsernameAsync(Input.Username);
                if (!userResult.Success || userResult.Data == null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập không tồn tại.");
                    return Page();
                }

                var user = userResult.Data;

                // Kiểm tra số điện thoại có khớp không
                if (user.PhoneNumber != Input.PhoneNumber)
                {
                    ModelState.AddModelError(string.Empty, "Số điện thoại không khớp với tài khoản.");
                    return Page();
                }

                // Đặt lại mật khẩu
                var resetPasswordDto = new ResetPasswordDto
                {
                    UserId = user.Id,
                    NewPassword = Input.NewPassword
                };

                var result = await _userService.ResetPasswordAsync(resetPasswordDto);
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "Không thể đặt lại mật khẩu.");
                    return Page();
                }

                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập với mật khẩu mới.";
                return RedirectToPage("/Account/Login");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đặt lại mật khẩu. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
