using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Reports
{
    public class ExportModel : PageModel
    {
        private readonly IReportingService _reportingService;

        public ExportModel(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        public async Task<IActionResult> OnGetAsync(
            DateTime startDate,
            DateTime endDate,
            string format = "excel",
            Guid? dealerId = null)
        {
            // Check permissions
            var userType = HttpContext.Session.GetUserType();
            var canExport = userType == UserType.Admin || userType == UserType.EVMStaff || 
                           userType == UserType.DealerManager;

            if (!canExport)
            {
                return Forbid();
            }

            try
            {
                var result = await _reportingService.ExportSalesReportAsync(
                    startDate, endDate, format, dealerId);

                if (result.Success && result.Data != null)
                {
                    var fileName = $"SalesReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.{format}";
                    var contentType = format.ToLower() switch
                    {
                        "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "pdf" => "application/pdf",
                        "csv" => "text/csv",
                        _ => "application/octet-stream"
                    };

                    return File(result.Data, contentType, fileName);
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Không thể xuất báo cáo.";
                    return RedirectToPage("/Reports/Index");
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error exporting report: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xuất báo cáo.";
                return RedirectToPage("/Reports/Index");
            }
        }
    }
}
