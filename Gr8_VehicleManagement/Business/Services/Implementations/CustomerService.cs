using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Entities;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ApplicationDbContext context, ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<CustomerDto>>> GetAllCustomersAsync(
            string? searchTerm = null,
            CustomerType? customerType = null,
            string? city = null,
            Guid? assignedUserId = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Customers.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(c =>
                        c.FullName.Contains(searchTerm) ||
                        c.PhoneNumber.Contains(searchTerm) ||
                        c.Id.ToString().Contains(searchTerm) ||
                        (c.Email != null && c.Email.Contains(searchTerm)));
                }

                if (customerType.HasValue)
                {
                    query = query.Where(c => c.CustomerType == customerType.Value);
                }

                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(c => c.City == city);
                }

                if (assignedUserId.HasValue)
                {
                    query = query.Where(c => c.AssignedUserId == assignedUserId.Value);
                }

                // Apply pagination
                var customers = await query
                    .OrderBy(c => c.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var customerDtos = new List<CustomerDto>();
                foreach (var customer in customers)
                {
                    var orderCount = await _context.Orders.CountAsync(o => o.CustomerId == customer.Id);
                    var totalSpent = await _context.Orders
                        .Where(o => o.CustomerId == customer.Id && o.Status != OrderStatus.Cancelled)
                        .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                    var testDriveCount = 0;

                    DateTime? lastInteraction = null;

                    var assignedUser = customer.AssignedUserId.HasValue
                        ? await _context.Users.FindAsync(customer.AssignedUserId.Value)
                        : null;

                    customerDtos.Add(new CustomerDto
                    {
                        Id = customer.Id,
                        Code = $"CUS{customer.Id.ToString().Substring(0, 6).ToUpper()}",
                        FullName = customer.FullName,
                        PhoneNumber = customer.PhoneNumber,
                        Email = customer.Email,
                        Address = customer.Address,
                        City = customer.City,
                        CustomerType = customer.CustomerType,
                        Source = customer.Source,
                        AssignedSalesperson = assignedUser?.FullName,
                        AssignedUserId = customer.AssignedUserId,
                        TotalOrders = orderCount,
                        TotalSpent = totalSpent,
                        TestDriveCount = testDriveCount,
                        LastInteractionDate = lastInteraction,
                        CreatedAt = customer.CreatedAt,
                        UpdatedAt = customer.UpdatedAt
                    });
                }

                return ServiceResponse<List<CustomerDto>>.SuccessResponse(customerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customers");
                return ServiceResponse<List<CustomerDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách khách hàng");
            }
        }

        public async Task<ServiceResponse<CustomerDetailDto>> GetCustomerByIdAsync(Guid id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return ServiceResponse<CustomerDetailDto>.ErrorResponse("Không tìm thấy khách hàng");
                }

                return await GetCustomerDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by ID");
                return ServiceResponse<CustomerDetailDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<CustomerDto>> GetCustomerByUserIdAsync(Guid userId)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.AssignedUserId == userId);

                if (customer == null)
                {
                    return ServiceResponse<CustomerDto>.ErrorResponse("Không tìm thấy khách hàng");
                }

                var customerDto = new CustomerDto
                {
                    Id = customer.Id,
                    Code = $"CUS{customer.Id.ToString().Substring(0, 6).ToUpper()}",
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    City = customer.City,
                    CustomerType = customer.CustomerType,
                    Source = customer.Source,
                    AssignedUserId = customer.AssignedUserId,
                    CreatedAt = customer.CreatedAt,
                    UpdatedAt = customer.UpdatedAt
                };

                return ServiceResponse<CustomerDto>.SuccessResponse(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer by user ID");
                return ServiceResponse<CustomerDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<CustomerDetailDto>> GetCustomerDetailsAsync(Guid id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return ServiceResponse<CustomerDetailDto>.ErrorResponse("Không tìm thấy khách hàng");
                }

                var assignedUser = customer.AssignedUserId.HasValue
                    ? await _context.Users.FindAsync(customer.AssignedUserId.Value)
                    : null;

                // Parse interactions
                var interactions = new List<CustomerInteractionDto>();

                // Parse test drives
                var testDrives = new List<TestDriveDto>();

                // Get orders
                var orders = await _context.Orders
                    .Include(o => o.VehicleVersion)
                    .ThenInclude(v => v.Model)
                    .Where(o => o.CustomerId == id)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                var orderDtos = orders.Select(o => new CustomerOrderDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    OrderDate = o.OrderDate,
                    VehicleName = $"{o.VehicleVersion.Model.Name} - {o.VehicleVersion.VersionName}",
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    PaymentStatus = o.PaymentStatus.ToString()
                }).ToList();

                var detailDto = new CustomerDetailDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    City = customer.City,
                    CustomerType = customer.CustomerType,
                    Source = customer.Source,
                    AssignedSalesperson = assignedUser?.FullName,
                    AssignedUserId = customer.AssignedUserId,
                    TotalOrders = orders.Count,
                    TotalSpent = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount),
                    TestDriveCount = testDrives.Count,
                    LastInteractionDate = interactions.FirstOrDefault()?.Date,
                    CreatedAt = customer.CreatedAt,
                    UpdatedAt = customer.UpdatedAt,
                    RecentInteractions = interactions.Take(10).ToList(),
                    TestDrives = testDrives,
                    Orders = orderDtos
                };

                return ServiceResponse<CustomerDetailDto>.SuccessResponse(detailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer details");
                return ServiceResponse<CustomerDetailDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerDto dto, Guid createdByUserId)
        {
            try
            {
                // Note: Phone number check removed for registration flow
                // since user and customer are created together

                var customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    Address = dto.Address,
                    City = dto.City,
                    CustomerType = dto.CustomerType,
                    Source = dto.Source,
                    AssignedUserId = dto.AssignedUserId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created customer: {Name}", customer.FullName);

                // Map to CustomerDto
                var customerDto = new CustomerDto
                {
                    Id = customer.Id,
                    FullName = customer.FullName,
                    PhoneNumber = customer.PhoneNumber,
                    Email = customer.Email,
                    Address = customer.Address,
                    City = customer.City,
                    CustomerType = customer.CustomerType,
                    Source = customer.Source,
                    AssignedUserId = customer.AssignedUserId,
                    CreatedAt = customer.CreatedAt
                };

                return ServiceResponse<CustomerDto>.SuccessResponse(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return ServiceResponse<CustomerDto>.ErrorResponse("Đã xảy ra lỗi khi tạo khách hàng");
            }
        }

        public async Task<ServiceResponse<CustomerDto>> UpdateCustomerAsync(UpdateCustomerDto dto, Guid updatedByUserId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(dto.Id);
                if (customer == null)
                {
                    return ServiceResponse<CustomerDto>.ErrorResponse("Không tìm thấy khách hàng");
                }

                // Check if phone number exists (except current customer)
                if (await _context.Customers.AnyAsync(c => c.PhoneNumber == dto.PhoneNumber && c.Id != dto.Id))
                {
                    return ServiceResponse<CustomerDto>.ErrorResponse("Số điện thoại đã tồn tại");
                }

                customer.FullName = dto.FullName;
                customer.PhoneNumber = dto.PhoneNumber;
                customer.Email = dto.Email;
                customer.Address = dto.Address;
                customer.City = dto.City;
                customer.CustomerType = dto.CustomerType;
                customer.Source = dto.Source;
                customer.AssignedUserId = dto.AssignedUserId;
                customer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated customer: {Name}", customer.FullName);

                var result = await GetCustomerByIdAsync(customer.Id);
                return new ServiceResponse<CustomerDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer");
                return ServiceResponse<CustomerDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật khách hàng");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCustomerAsync(Guid id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy khách hàng");
                }

                // Check if customer has orders
                var hasOrders = await _context.Orders.AnyAsync(o => o.CustomerId == id);
                if (hasOrders)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không thể xóa khách hàng đã có đơn hàng");
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted customer: {Name}", customer.FullName);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xóa khách hàng");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalCustomersCountAsync()
        {
            try
            {
                var count = await _context.Customers.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total customers count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<List<string>>> GetCitiesAsync()
        {
            try
            {
                var cities = await _context.Customers
                    .Where(c => c.City != null)
                    .Select(c => c.City!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                return ServiceResponse<List<string>>.SuccessResponse(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cities");
                return ServiceResponse<List<string>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetCustomerOrderCountAsync(Guid customerId)
        {
            try
            {
                var count = await _context.Orders.CountAsync(o => o.CustomerId == customerId);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer order count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetCustomerTotalSpentAsync(Guid customerId)
        {
            try
            {
                var total = await _context.Orders
                    .Where(o => o.CustomerId == customerId && o.Status != OrderStatus.Cancelled)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

                return ServiceResponse<decimal>.SuccessResponse(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer total spent");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<bool>> AssignCustomerToSalespersonAsync(Guid customerId, Guid userId)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy khách hàng");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy nhân viên");
                }

                customer.AssignedUserId = userId;
                customer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Assigned customer {CustomerId} to user {UserId}", customerId, userId);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning customer");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

    }
}

