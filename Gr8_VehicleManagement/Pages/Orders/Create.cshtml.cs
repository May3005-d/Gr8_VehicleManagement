using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gr8_VehicleManagement.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IVehicleService _vehicleService;

        public CreateModel(
            IOrderService orderService,
            ICustomerService customerService,
            IVehicleService vehicleService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _vehicleService = vehicleService;
        }

        [BindProperty]
        public CreateOrderInput Input { get; set; } = new CreateOrderInput();

        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public List<VehicleVersionDto> VehicleVersions { get; set; } = new List<VehicleVersionDto>();
        public List<DealerDto> Dealers { get; set; } = new List<DealerDto>();

        public class CreateOrderInput
        {
            public Guid CustomerId { get; set; }
            public Guid VersionId { get; set; }
            public Guid DealerId { get; set; }
            public decimal BasePrice { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal DepositAmount { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
            public DateTime? ExpectedDeliveryDate { get; set; }
            public string? InstallmentProvider { get; set; }
            public int? InstallmentTerm { get; set; }
            public decimal? MonthlyPayment { get; set; }
            public decimal? InterestRate { get; set; }
            public string? Notes { get; set; }
        }

        public class VehicleVersionDto
        {
            public Guid Id { get; set; }
            public string ModelName { get; set; } = string.Empty;
            public string VersionName { get; set; } = string.Empty;
            public string ColorName { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }

        public class DealerDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userType = HttpContext.Session.GetUserType();
            if (userType != UserType.Admin && userType != UserType.EVMStaff &&
                userType != UserType.DealerManager && userType != UserType.DealerStaff)
            {
                return Forbid();
            }

            await LoadDataAsync();
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
                await LoadDataAsync();
                return Page();
            }

            try
            {
                var userId = HttpContext.Session.GetUserId();
                if (userId == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy thông tin người dùng");
                    await LoadDataAsync();
                    return Page();
                }

                var createDto = new CreateOrderDto
                {
                    CustomerId = Input.CustomerId,
                    VersionId = Input.VersionId,
                    DealerId = Input.DealerId,
                    CreatedBy = userId.Value,
                    BasePrice = Input.BasePrice,
                    DiscountAmount = Input.DiscountAmount,
                    TotalAmount = Input.TotalAmount,
                    DepositAmount = Input.DepositAmount,
                    PaymentMethod = Input.PaymentMethod,
                    ExpectedDeliveryDate = Input.ExpectedDeliveryDate,
                    InstallmentProvider = Input.InstallmentProvider,
                    InstallmentTerm = Input.InstallmentTerm,
                    MonthlyPayment = Input.MonthlyPayment,
                    InterestRate = Input.InterestRate,
                    Notes = Input.Notes
                };

                var result = await _orderService.CreateOrderAsync(createDto);
                if (result.Success)
                {
                    TempData["Message"] = "Tạo đơn hàng thành công!";
                    return RedirectToPage("/Orders/Details", new { id = result.Data?.Id });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    await LoadDataAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error creating order: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo đơn hàng");
                await LoadDataAsync();
                return Page();
            }
        }

        private async Task LoadDataAsync()
        {
            // Load customers
            var customersResult = await _customerService.GetAllCustomersAsync(pageNumber: 1, pageSize: 1000);
            if (customersResult.Success)
            {
                Customers = customersResult.Data ?? new List<CustomerDto>();
            }

            // Load vehicle versions (simplified - in real app, you'd have a proper service)
            VehicleVersions = new List<VehicleVersionDto>
            {
                new VehicleVersionDto { Id = Guid.NewGuid(), ModelName = "VinFast VF8", VersionName = "Standard", ColorName = "Trắng", Price = 890000000 },
                new VehicleVersionDto { Id = Guid.NewGuid(), ModelName = "VinFast VF8", VersionName = "Premium", ColorName = "Đen", Price = 950000000 },
                new VehicleVersionDto { Id = Guid.NewGuid(), ModelName = "VinFast VF9", VersionName = "Standard", ColorName = "Xám", Price = 1200000000 },
                new VehicleVersionDto { Id = Guid.NewGuid(), ModelName = "VinFast VF9", VersionName = "Premium", ColorName = "Xanh", Price = 1300000000 },
                new VehicleVersionDto { Id = Guid.NewGuid(), ModelName = "Tesla Model 3", VersionName = "Standard Range", ColorName = "Trắng", Price = 1500000000 },
                new VehicleVersionDto { Id = Guid.NewGuid(), ModelName = "Tesla Model 3", VersionName = "Long Range", ColorName = "Đen", Price = 1800000000 }
            };

            // Load dealers (simplified - in real app, you'd have a proper service)
            Dealers = new List<DealerDto>
            {
                new DealerDto { Id = Guid.NewGuid(), Name = "Đại lý Hà Nội", City = "Hà Nội" },
                new DealerDto { Id = Guid.NewGuid(), Name = "Đại lý TP.HCM", City = "TP.HCM" },
                new DealerDto { Id = Guid.NewGuid(), Name = "Đại lý Đà Nẵng", City = "Đà Nẵng" }
            };
        }
    }
}
