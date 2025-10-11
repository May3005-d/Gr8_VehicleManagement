using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Gr8_VehicleManagement.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;

        public RegisterModel(ICustomerService customerService, IUserService userService)
        {
            _customerService = customerService;
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

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


            [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
            [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
            [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            // Nếu đã đăng nhập, chuyển về dashboard
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
                // Kiểm tra username đã tồn tại chưa
                var existingUser = await _userService.GetUserByUsernameAsync(Input.Username);
                if (existingUser.Success && existingUser.Data != null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
                    return Page();
                }

                // Tạo user mới
                var createUserDto = new CreateUserDto
                {
                    Username = Input.Username,
                    Password = Input.Password,
                    FullName = Input.FullName,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                    UserType = UserType.Customer
                };

                var userResult = await _userService.CreateUserAsync(createUserDto);
                if (!userResult.Success)
                {
                    ModelState.AddModelError(string.Empty, userResult.Message ?? "Không thể tạo tài khoản.");
                    return Page();
                }

                // Tạo customer record
                var createCustomerDto = new CreateCustomerDto
                {
                    FullName = Input.FullName,
                    PhoneNumber = Input.PhoneNumber,
                    Email = Input.Email,
                    CustomerType = CustomerType.Individual,
                    AssignedUserId = userResult.Data?.Id
                };

                var customerResult = await _customerService.CreateCustomerAsync(createCustomerDto, userResult.Data!.Id);
                if (!customerResult.Success)
                {
                    // Nếu tạo customer thất bại, xóa user đã tạo
                    if (userResult.Data != null)
                    {
                        await _userService.DeleteUserAsync(userResult.Data.Id);
                    }
                    ModelState.AddModelError(string.Empty, $"Không thể tạo thông tin khách hàng: {customerResult.Message}");
                    return Page();
                }

                // Đăng nhập tự động sau khi đăng ký thành công
                HttpContext.Session.SetUserId(userResult.Data!.Id);
                HttpContext.Session.SetUserType(userResult.Data.UserType);
                HttpContext.Session.SetUsername(userResult.Data.Username);

                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Chào mừng bạn đến với GR8 Vehicle Management.";
                return RedirectToPage("/Dashboard/Customer");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.");
                return Page();
            }
        }
    }
}
