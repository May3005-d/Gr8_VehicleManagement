using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Entities;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<OrderDto>>> GetAllOrdersAsync(
            string? searchTerm = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            Guid? customerId = null,
            Guid? dealerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Dealer)
                    .Include(o => o.VehicleVersion)
                    .ThenInclude(v => v.Model)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(o =>
                        o.OrderCode.Contains(searchTerm) ||
                        o.Customer.FullName.Contains(searchTerm) ||
                        o.Customer.PhoneNumber.Contains(searchTerm) ||
                        (o.ContractNumber != null && o.ContractNumber.Contains(searchTerm)));
                }

                if (status.HasValue)
                {
                    query = query.Where(o => o.Status == status.Value);
                }

                if (paymentStatus.HasValue)
                {
                    query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
                }

                if (customerId.HasValue)
                {
                    query = query.Where(o => o.CustomerId == customerId.Value);
                }

                if (dealerId.HasValue)
                {
                    query = query.Where(o => o.DealerId == dealerId.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= toDate.Value);
                }

                // Apply pagination
                var orders = await query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var orderDtos = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    OrderDate = o.OrderDate,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.FullName,
                    CustomerPhone = o.Customer.PhoneNumber,
                    VersionId = o.VersionId,
                    VehicleName = $"{o.VehicleVersion.Model.Name} - {o.VehicleVersion.VersionName}",
                    VehicleColor = o.VehicleVersion.ColorName,
                    DealerId = o.DealerId,
                    DealerName = o.Dealer.Name,
                    BasePrice = o.BasePrice,
                    DiscountAmount = o.DiscountAmount,
                    TotalAmount = o.TotalAmount,
                    DepositAmount = o.DepositAmount,
                    RemainingAmount = o.RemainingAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    ExpectedDeliveryDate = o.ExpectedDeliveryDate,
                    ActualDeliveryDate = o.ActualDeliveryDate,
                    ContractNumber = o.ContractNumber,
                    ContractSignedAt = o.ContractSignedAt,
                    InstallmentProvider = o.InstallmentProvider,
                    InstallmentTerm = o.InstallmentTerm,
                    MonthlyPayment = o.MonthlyPayment,
                    InterestRate = o.InterestRate,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                }).ToList();

                return ServiceResponse<List<OrderDto>>.SuccessResponse(orderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders");
                return ServiceResponse<List<OrderDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách đơn hàng");
            }
        }

        public async Task<ServiceResponse<OrderDetailDto>> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Dealer)
                    .Include(o => o.VehicleVersion)
                    .ThenInclude(v => v.Model)
                    .Include(o => o.Payments)
                    .Include(o => o.Feedbacks)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return ServiceResponse<OrderDetailDto>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                var orderDetail = new OrderDetailDto
                {
                    Id = order.Id,
                    OrderCode = order.OrderCode,
                    OrderDate = order.OrderDate,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer.FullName,
                    CustomerPhone = order.Customer.PhoneNumber,
                    CustomerEmail = order.Customer.Email,
                    CustomerAddress = order.Customer.Address,
                    VersionId = order.VersionId,
                    VehicleName = $"{order.VehicleVersion.Model.Name} - {order.VehicleVersion.VersionName}",
                    VehicleColor = order.VehicleVersion.ColorName,
                    VehicleSpecs = $"{order.VehicleVersion.BatteryCapacity}kWh, {order.VehicleVersion.Range}km range",
                    DealerId = order.DealerId,
                    DealerName = order.Dealer.Name,
                    BasePrice = order.BasePrice,
                    DiscountAmount = order.DiscountAmount,
                    TotalAmount = order.TotalAmount,
                    DepositAmount = order.DepositAmount,
                    RemainingAmount = order.RemainingAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    PaymentMethod = order.PaymentMethod,
                    ExpectedDeliveryDate = order.ExpectedDeliveryDate,
                    ActualDeliveryDate = order.ActualDeliveryDate,
                    ContractNumber = order.ContractNumber,
                    ContractSignedAt = order.ContractSignedAt,
                    InstallmentProvider = order.InstallmentProvider,
                    InstallmentTerm = order.InstallmentTerm,
                    MonthlyPayment = order.MonthlyPayment,
                    InterestRate = order.InterestRate,
                    DeliveryNotes = order.DeliveryNotes,
                    CancellationReason = order.CancellationReason,
                    Notes = order.Notes,
                    DeliveredAt = order.DeliveredAt,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    Payments = order.Payments.Select(p => new PaymentDto
                    {
                        Id = p.Id,
                        PaymentNumber = p.PaymentNumber,
                        Amount = p.Amount,
                        PaymentType = p.PaymentType,
                        PaymentMethodType = p.PaymentMethodType,
                        Status = p.Status,
                        PaymentDate = p.PaymentDate,
                        TransactionId = p.TransactionId,
                        PaymentProof = p.PaymentProof,
                        Notes = p.Notes
                    }).ToList(),
                    Feedbacks = order.Feedbacks.Select(f => new FeedbackDto
                    {
                        Id = f.Id,
                        FeedbackType = f.Type, // Changed from Type
                        Title = f.Title, // Changed from Subject
                        Description = f.Description, // Changed from Content
                        Status = f.Status,
                        Priority = f.Priority,
                        CreatedAt = f.CreatedAt,
                        ResolvedAt = f.ResolvedAt,
                        Resolution = f.Resolution
                    }).ToList()
                };

                return ServiceResponse<OrderDetailDto>.SuccessResponse(orderDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by ID");
                return ServiceResponse<OrderDetailDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto)
        {
            try
            {
                // Generate order code
                var count = await _context.Orders.CountAsync();
                var orderCode = $"ORD{(count + 1):D6}";

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderCode = orderCode,
                    CustomerId = dto.CustomerId,
                    DealerId = dto.DealerId,
                    CreatedBy = dto.CreatedBy,
                    VersionId = dto.VersionId,
                    BasePrice = dto.BasePrice,
                    DiscountAmount = dto.DiscountAmount,
                    TotalAmount = dto.TotalAmount,
                    DepositAmount = dto.DepositAmount,
                    RemainingAmount = dto.TotalAmount - dto.DepositAmount,
                    PaymentMethod = dto.PaymentMethod,
                    ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
                    InstallmentProvider = dto.InstallmentProvider,
                    InstallmentTerm = dto.InstallmentTerm,
                    MonthlyPayment = dto.MonthlyPayment,
                    InterestRate = dto.InterestRate,
                    Notes = dto.Notes,
                    Status = OrderStatus.Draft,
                    PaymentStatus = PaymentStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created order: {OrderCode}", order.OrderCode);

                var result = await GetOrderByIdAsync(order.Id);
                return new ServiceResponse<OrderDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return ServiceResponse<OrderDto>.ErrorResponse("Đã xảy ra lỗi khi tạo đơn hàng");
            }
        }

        public async Task<ServiceResponse<OrderDto>> UpdateOrderAsync(UpdateOrderDto dto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(dto.Id);
                if (order == null)
                {
                    return ServiceResponse<OrderDto>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                order.Status = dto.Status;
                order.PaymentStatus = dto.PaymentStatus;
                order.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
                order.ActualDeliveryDate = dto.ActualDeliveryDate;
                order.ContractNumber = dto.ContractNumber;
                order.ContractSignedAt = dto.ContractSignedAt;
                order.DeliveredAt = dto.DeliveredAt;
                order.DeliveryNotes = dto.DeliveryNotes;
                order.CancellationReason = dto.CancellationReason;
                order.Notes = dto.Notes;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated order: {OrderCode}", order.OrderCode);

                var result = await GetOrderByIdAsync(order.Id);
                return new ServiceResponse<OrderDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return ServiceResponse<OrderDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật đơn hàng");
            }
        }

        public async Task<ServiceResponse<bool>> CancelOrderAsync(Guid id, string reason)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                if (order.Status == OrderStatus.Delivered)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không thể hủy đơn hàng đã giao");
                }

                order.Status = OrderStatus.Cancelled;
                order.CancellationReason = reason;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cancelled order: {OrderCode}", order.OrderCode);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi hủy đơn hàng");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatusAsync(Guid id, OrderStatus status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated order status: {OrderCode} to {Status}", order.OrderCode, status);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi cập nhật trạng thái");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePaymentStatusAsync(Guid id, PaymentStatus status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                order.PaymentStatus = status;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated payment status: {OrderCode} to {Status}", order.OrderCode, status);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi cập nhật trạng thái thanh toán");
            }
        }

        public async Task<ServiceResponse<bool>> MarkAsDeliveredAsync(Guid id, string? deliveryNotes = null)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                order.Status = OrderStatus.Delivered;
                order.ActualDeliveryDate = DateTime.UtcNow;
                order.DeliveredAt = DateTime.UtcNow;
                order.DeliveryNotes = deliveryNotes;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Marked order as delivered: {OrderCode}", order.OrderCode);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking order as delivered");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi cập nhật giao hàng");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalOrdersCountAsync()
        {
            try
            {
                var count = await _context.Orders.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total orders count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.Orders.Where(o => o.Status != OrderStatus.Cancelled);

                if (fromDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= toDate.Value);
                }

                var total = await query.SumAsync(o => o.TotalAmount);
                return ServiceResponse<decimal>.SuccessResponse(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetOrdersCountByStatusAsync(OrderStatus status)
        {
            try
            {
                var count = await _context.Orders.CountAsync(o => o.Status == status);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders count by status");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetRemainingAmountAsync(Guid orderId)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return ServiceResponse<decimal>.ErrorResponse("Không tìm thấy đơn hàng");
                }

                var totalPaid = await _context.Payments
                    .Where(p => p.OrderId == orderId && p.Status == PaymentStatus.PaidInFull)
                    .SumAsync(p => p.Amount);

                var remaining = order.TotalAmount - totalPaid;
                return ServiceResponse<decimal>.SuccessResponse(remaining);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting remaining amount");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }
    }
}

