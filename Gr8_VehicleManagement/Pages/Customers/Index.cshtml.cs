using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Pages.Customers
{
    public class IndexModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly Data.ApplicationDbContext _context;

        public IndexModel(ICustomerService customerService, Data.ApplicationDbContext context)
        {
            _customerService = customerService;
            _context = context;
        }

        public List<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public CustomerType? CustomerType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? City { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? AssignedUserId { get; set; }

        public List<string> Cities { get; set; } = new List<string>();
        public List<UserDto> Salespeople { get; set; } = new List<UserDto>();

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

            // Get cities for filter
            var citiesResult = await _customerService.GetCitiesAsync();
            if (citiesResult.Success)
            {
                Cities = citiesResult.Data ?? new List<string>();
            }

            // Get salespeople for filter
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

            // Get customers
            var result = await _customerService.GetAllCustomersAsync(
                searchTerm: SearchTerm,
                customerType: CustomerType,
                city: City,
                assignedUserId: AssignedUserId,
                pageNumber: PageNumber,
                pageSize: PageSize);

            if (result.Success)
            {
                Customers = result.Data ?? new List<CustomerDto>();

                var totalResult = await _customerService.GetTotalCustomersCountAsync();
                if (totalResult.Success)
                {
                    TotalCount = totalResult.Data;
                }
            }

            return Page();
        }
    }
}

