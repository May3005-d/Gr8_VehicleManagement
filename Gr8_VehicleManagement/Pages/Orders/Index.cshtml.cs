using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;

        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public OrderStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public PaymentStatus? PaymentStatus { get; set; }

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

            // Get orders
            var result = await _orderService.GetAllOrdersAsync(
                searchTerm: SearchTerm,
                status: Status,
                paymentStatus: PaymentStatus,
                customerId: null,
                dealerId: null,
                fromDate: FromDate,
                toDate: ToDate,
                pageNumber: PageNumber,
                pageSize: PageSize);

            if (result.Success)
            {
                Orders = result.Data ?? new List<OrderDto>();

                var totalResult = await _orderService.GetTotalOrdersCountAsync();
                if (totalResult.Success)
                {
                    TotalCount = totalResult.Data;
                }
            }

            return Page();
        }
    }
}

