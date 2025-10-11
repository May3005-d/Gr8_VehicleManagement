using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Gr8_VehicleManagement.Pages.Customers
{
    public class CreateModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly Data.ApplicationDbContext _context;

        public CreateModel(ICustomerService customerService, Data.ApplicationDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<UserDto> Salespeople { get; set; } = new List<UserDto>();

        public class InputModel
        {
            [Required(ErrorMessage = "Họ tên là bắt buộc")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            public string PhoneNumber { get; set; } = string.Empty;

            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string? Email { get; set; }

            public string? Address { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }
            public string? IDNumber { get; set; }

            [Required(ErrorMessage = "Loại khách hàng là bắt buộc")]
            public CustomerType CustomerType { get; set; }

            public string? Source { get; set; }
            public Guid? AssignedUserId { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            await LoadSalespeople();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await LoadSalespeople();
                return Page();
            }

            var userId = HttpContext.Session.GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToPage("/Account/Login");
            }

            var dto = new CreateCustomerDto
            {
                FullName = Input.FullName,
                PhoneNumber = Input.PhoneNumber,
                Email = Input.Email,
                Address = Input.Address,
                City = Input.City,
                CustomerType = Input.CustomerType,
                Source = Input.Source,
                AssignedUserId = Input.AssignedUserId
            };

            var result = await _customerService.CreateCustomerAsync(dto, userId.Value);

            if (result.Success && result.Data != null)
            {
                TempData["Message"] = "Đã thêm khách hàng thành công!";
                return RedirectToPage("/Customers/Details", new { id = result.Data.Id });
            }

            ModelState.AddModelError(string.Empty, result.Message);
            await LoadSalespeople();
            return Page();
        }

        private async Task LoadSalespeople()
        {
            var salespeople = await _context.Users
                .Where(u => u.UserType == UserType.DealerStaff || u.UserType == UserType.DealerManager)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            Salespeople = salespeople.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            }).ToList();
        }
    }
}

