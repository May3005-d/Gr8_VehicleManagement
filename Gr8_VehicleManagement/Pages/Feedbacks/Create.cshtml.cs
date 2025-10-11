using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Feedbacks
{
    public class CreateModel : PageModel
    {
        private readonly IFeedbackService _feedbackService;

        public CreateModel(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [BindProperty]
        public CreateFeedbackDto Input { get; set; } = new CreateFeedbackDto();

        public bool CanCreate { get; set; }

        public IActionResult OnGet()
        {
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff || userType == UserType.Customer;

            if (!CanCreate)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToPage("/AccessDenied");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff || userType == UserType.Customer;

            if (!CanCreate)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thực hiện hành động này.";
                return RedirectToPage("/AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _feedbackService.CreateFeedbackAsync(Input);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Phản hồi đã được gửi thành công! Chúng tôi sẽ xử lý trong thời gian sớm nhất.";
                return RedirectToPage("/Feedbacks/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Đã xảy ra lỗi khi gửi phản hồi.");
                return Page();
            }
        }
    }
}
