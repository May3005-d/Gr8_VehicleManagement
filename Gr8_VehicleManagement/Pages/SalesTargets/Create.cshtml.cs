using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Pages.SalesTargets
{
    public class CreateModel : PageModel
    {
        private readonly ISalesTargetService _salesTargetService;
        private readonly ApplicationDbContext _context;

        public CreateModel(ISalesTargetService salesTargetService, ApplicationDbContext context)
        {
            _salesTargetService = salesTargetService;
            _context = context;
        }

        [BindProperty]
        public CreateSalesTargetDto Input { get; set; } = new CreateSalesTargetDto();

        public bool CanCreate { get; set; }
        public List<DealerDto> Dealers { get; set; } = new List<DealerDto>();
        public List<UserDto> Users { get; set; } = new List<UserDto>();

        public IActionResult OnGet()
        {
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff;

            if (!CanCreate)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/AccessDenied");
            }

            LoadDealersAndUsers();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff;

            if (!CanCreate)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này.";
                return RedirectToPage("/AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                LoadDealersAndUsers();
                return Page();
            }

            // Validate target assignment
            if (Input.TargetFor == TargetFor.Dealer && !Input.DealerId.HasValue)
            {
                ModelState.AddModelError("Input.DealerId", "Phải chọn đại lý khi chỉ tiêu dành cho đại lý.");
                LoadDealersAndUsers();
                return Page();
            }

            if (Input.TargetFor == TargetFor.Staff && !Input.UserId.HasValue)
            {
                ModelState.AddModelError("Input.UserId", "Phải chọn nhân viên khi chỉ tiêu dành cho nhân viên.");
                LoadDealersAndUsers();
                return Page();
            }

            var result = await _salesTargetService.CreateSalesTargetAsync(Input);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Chỉ tiêu bán hàng đã được tạo thành công!";
                return RedirectToPage("/SalesTargets/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Đã xảy ra lỗi khi tạo chỉ tiêu bán hàng.");
                LoadDealersAndUsers();
                return Page();
            }
        }

        private void LoadDealersAndUsers()
        {
            try
            {
                // Load dealers
                Dealers = _context.Dealers
                    .Select(d => new DealerDto
                    {
                        Id = d.Id,
                        Name = d.Name
                    })
                    .ToList();

                // Load users (staff only)
                Users = _context.Users
                    .Where(u => u.UserType == UserType.DealerStaff || u.UserType == UserType.DealerManager)
                    .OrderBy(u => u.CreatedAt)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        FullName = u.FullName
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading dealers and users: {ex.Message}");
            }
        }
    }

    public class DealerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
