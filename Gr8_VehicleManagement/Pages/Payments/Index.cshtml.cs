using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Payments
{
    public class IndexModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public IndexModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public List<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public PaymentStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public PaymentType? PaymentType { get; set; }

        [BindProperty(SupportsGet = true)]
        public PaymentMethodType? PaymentMethodType { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            var userType = HttpContext.Session.GetUserType();
            CanCreate = userType == UserType.Admin || userType == UserType.EVMStaff ||
                        userType == UserType.DealerManager || userType == UserType.DealerStaff;
            CanEdit = CanCreate;

            if (pageNumber.HasValue && pageNumber.Value > 0)
            {
                PageNumber = pageNumber.Value;
            }

            // Get payments
            var result = await _paymentService.GetAllPaymentsAsync(
                searchTerm: SearchTerm,
                status: Status,
                paymentType: PaymentType,
                paymentMethodType: PaymentMethodType,
                orderId: null,
                customerId: null,
                fromDate: FromDate,
                toDate: ToDate,
                pageNumber: PageNumber,
                pageSize: PageSize);

            if (result.Success)
            {
                Payments = result.Data ?? new List<PaymentDto>();

                var totalResult = await _paymentService.GetTotalPaymentsCountAsync();
                if (totalResult.Success)
                {
                    TotalCount = totalResult.Data;
                }
            }

            return Page();
        }
    }
}
