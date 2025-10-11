using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;

namespace Gr8_VehicleManagement.Business.Services.Interfaces
{
    public interface ICustomerService
    {
        // List & Search
        Task<ServiceResponse<List<CustomerDto>>> GetAllCustomersAsync(
            string? searchTerm = null,
            CustomerType? customerType = null,
            string? city = null,
            Guid? assignedUserId = null,
            int pageNumber = 1,
            int pageSize = 10);
        
        Task<ServiceResponse<int>> GetTotalCustomersCountAsync();
        Task<ServiceResponse<List<string>>> GetCitiesAsync();
        
        // CRUD
        Task<ServiceResponse<CustomerDetailDto>> GetCustomerByIdAsync(Guid id);
        Task<ServiceResponse<CustomerDto>> GetCustomerByUserIdAsync(Guid userId);
        Task<ServiceResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto dto, Guid createdByUserId);
        Task<ServiceResponse<CustomerDto>> UpdateCustomerAsync(UpdateCustomerDto dto, Guid updatedByUserId);
        Task<ServiceResponse<bool>> DeleteCustomerAsync(Guid id);
        
        // Statistics
        Task<ServiceResponse<CustomerDetailDto>> GetCustomerDetailsAsync(Guid id);
        Task<ServiceResponse<int>> GetCustomerOrderCountAsync(Guid customerId);
        Task<ServiceResponse<decimal>> GetCustomerTotalSpentAsync(Guid customerId);
        
        // Assignment
        Task<ServiceResponse<bool>> AssignCustomerToSalespersonAsync(Guid customerId, Guid userId);
    }
}

