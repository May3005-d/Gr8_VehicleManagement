using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Entities;
using Gr8_VehicleManagement.Data.Enums;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<PaymentDto>>> GetAllPaymentsAsync(
            string? searchTerm = null,
            PaymentStatus? status = null,
            PaymentType? paymentType = null,
            PaymentMethodType? paymentMethodType = null,
            Guid? orderId = null,
            Guid? customerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Payments
                    .Include(p => p.Order)
                    .Include(p => p.Customer)
                    .Include(p => p.Processor)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p =>
                        p.PaymentNumber.Contains(searchTerm) ||
                        p.Customer.FullName.Contains(searchTerm) ||
                        p.Customer.PhoneNumber.Contains(searchTerm) ||
                        (p.TransactionId != null && p.TransactionId.Contains(searchTerm)));
                }

                if (status.HasValue)
                {
                    query = query.Where(p => p.Status == status.Value);
                }

                if (paymentType.HasValue)
                {
                    query = query.Where(p => p.PaymentType == paymentType.Value);
                }

                if (paymentMethodType.HasValue)
                {
                    query = query.Where(p => p.PaymentMethodType == paymentMethodType.Value);
                }

                if (orderId.HasValue)
                {
                    query = query.Where(p => p.OrderId == orderId.Value);
                }

                if (customerId.HasValue)
                {
                    query = query.Where(p => p.CustomerId == customerId.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(p => p.PaymentDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(p => p.PaymentDate <= toDate.Value);
                }

                // Apply pagination
                var payments = await query
                    .OrderByDescending(p => p.PaymentDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var paymentDtos = payments.Select(p => new PaymentDto
                {
                    Id = p.Id,
                    PaymentNumber = p.PaymentNumber,
                    PaymentDate = p.PaymentDate,
                    OrderId = p.OrderId,
                    OrderCode = p.Order.OrderCode,
                    CustomerId = p.CustomerId,
                    CustomerName = p.Customer.FullName,
                    CustomerPhone = p.Customer.PhoneNumber,
                    Amount = p.Amount,
                    PaymentType = p.PaymentType,
                    PaymentMethodType = p.PaymentMethodType,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentProof = p.PaymentProof,
                    InstallmentNumber = p.InstallmentNumber,
                    DueDate = p.DueDate,
                    LateFee = p.LateFee,
                    RefundReason = p.RefundReason,
                    RefundApprovedAt = p.RefundApprovedAt,
                    ProcessedBy = p.ProcessedBy,
                    ProcessorName = p.Processor?.FullName,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return ServiceResponse<List<PaymentDto>>.SuccessResponse(paymentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments");
                return ServiceResponse<List<PaymentDto>>.ErrorResponse("Đã xảy ra lỗi khi tải danh sách thanh toán");
            }
        }

        public async Task<ServiceResponse<PaymentDetailDto>> GetPaymentByIdAsync(Guid id)
        {
            try
            {
                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .ThenInclude(o => o.VehicleVersion)
                    .ThenInclude(v => v.Model)
                    .Include(p => p.Customer)
                    .Include(p => p.Processor)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (payment == null)
                {
                    return ServiceResponse<PaymentDetailDto>.ErrorResponse("Không tìm thấy thanh toán");
                }

                var paymentDetail = new PaymentDetailDto
                {
                    Id = payment.Id,
                    PaymentNumber = payment.PaymentNumber,
                    PaymentDate = payment.PaymentDate,
                    OrderId = payment.OrderId,
                    OrderCode = payment.Order.OrderCode,
                    CustomerId = payment.CustomerId,
                    CustomerName = payment.Customer.FullName,
                    CustomerPhone = payment.Customer.PhoneNumber,
                    CustomerEmail = payment.Customer.Email,
                    CustomerAddress = payment.Customer.Address,
                    Amount = payment.Amount,
                    PaymentType = payment.PaymentType,
                    PaymentMethodType = payment.PaymentMethodType,
                    Status = payment.Status,
                    TransactionId = payment.TransactionId,
                    PaymentProof = payment.PaymentProof,
                    InstallmentNumber = payment.InstallmentNumber,
                    DueDate = payment.DueDate,
                    LateFee = payment.LateFee,
                    RefundReason = payment.RefundReason,
                    RefundApprovedAt = payment.RefundApprovedAt,
                    ProcessedBy = payment.ProcessedBy,
                    ProcessorName = payment.Processor?.FullName,
                    Notes = payment.Notes,
                    CreatedAt = payment.CreatedAt,
                    UpdatedAt = payment.UpdatedAt,
                    OrderDetails = $"{payment.Order.VehicleVersion.Model.Name} - {payment.Order.VehicleVersion.VersionName}",
                    VehicleInfo = $"{payment.Order.VehicleVersion.Model.Name} - {payment.Order.VehicleVersion.VersionName}",
                    TotalOrderAmount = payment.Order.TotalAmount,
                    RemainingAmount = payment.Order.RemainingAmount
                };

                return ServiceResponse<PaymentDetailDto>.SuccessResponse(paymentDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment by ID");
                return ServiceResponse<PaymentDetailDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<PaymentDto>> CreatePaymentAsync(CreatePaymentDto dto)
        {
            try
            {
                // Generate payment number
                var count = await _context.Payments.CountAsync();
                var paymentNumber = $"PAY{(count + 1):D6}";

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    PaymentNumber = paymentNumber,
                    OrderId = dto.OrderId,
                    CustomerId = dto.CustomerId,
                    Amount = dto.Amount,
                    PaymentType = dto.PaymentType,
                    PaymentMethodType = dto.PaymentMethodType,
                    Status = PaymentStatus.Pending,
                    PaymentDate = dto.PaymentDate,
                    TransactionId = dto.TransactionId,
                    PaymentProof = dto.PaymentProof,
                    InstallmentNumber = dto.InstallmentNumber,
                    DueDate = dto.DueDate,
                    LateFee = dto.LateFee,
                    RefundReason = dto.RefundReason,
                    ProcessedBy = dto.ProcessedBy,
                    Notes = dto.Notes,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created payment: {PaymentNumber}", payment.PaymentNumber);

                var result = await GetPaymentByIdAsync(payment.Id);
                return new ServiceResponse<PaymentDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return ServiceResponse<PaymentDto>.ErrorResponse("Đã xảy ra lỗi khi tạo thanh toán");
            }
        }

        public async Task<ServiceResponse<PaymentDto>> UpdatePaymentAsync(UpdatePaymentDto dto)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(dto.Id);
                if (payment == null)
                {
                    return ServiceResponse<PaymentDto>.ErrorResponse("Không tìm thấy thanh toán");
                }

                payment.Status = dto.Status;
                payment.TransactionId = dto.TransactionId;
                payment.PaymentProof = dto.PaymentProof;
                payment.LateFee = dto.LateFee;
                payment.RefundReason = dto.RefundReason;
                payment.RefundApprovedAt = dto.RefundApprovedAt;
                payment.Notes = dto.Notes;
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated payment: {PaymentNumber}", payment.PaymentNumber);

                var result = await GetPaymentByIdAsync(payment.Id);
                return new ServiceResponse<PaymentDto>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment");
                return ServiceResponse<PaymentDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật thanh toán");
            }
        }

        public async Task<ServiceResponse<bool>> DeletePaymentAsync(Guid id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy thanh toán");
                }

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted payment: {PaymentNumber}", payment.PaymentNumber);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xóa thanh toán");
            }
        }

        public async Task<ServiceResponse<bool>> UpdatePaymentStatusAsync(Guid id, PaymentStatus status)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy thanh toán");
                }

                payment.Status = status;
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated payment status: {PaymentNumber} to {Status}", payment.PaymentNumber, status);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment status");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi cập nhật trạng thái");
            }
        }

        public async Task<ServiceResponse<bool>> ApproveRefundAsync(Guid id, string reason)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy thanh toán");
                }

                payment.Status = PaymentStatus.Refunded;
                payment.RefundReason = reason;
                payment.RefundApprovedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Approved refund for payment: {PaymentNumber}", payment.PaymentNumber);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving refund");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi phê duyệt hoàn tiền");
            }
        }

        public async Task<ServiceResponse<bool>> ProcessPaymentAsync(Guid id, string transactionId)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy thanh toán");
                }

                payment.Status = PaymentStatus.PaidInFull;
                payment.TransactionId = transactionId;
                payment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Processed payment: {PaymentNumber}", payment.PaymentNumber);

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xử lý thanh toán");
            }
        }

        public async Task<ServiceResponse<int>> GetTotalPaymentsCountAsync()
        {
            try
            {
                var count = await _context.Payments.CountAsync();
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total payments count");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.Payments.Where(p => p.Status == PaymentStatus.PaidInFull);

                if (fromDate.HasValue)
                {
                    query = query.Where(p => p.PaymentDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(p => p.PaymentDate <= toDate.Value);
                }

                var total = await query.SumAsync(p => p.Amount);
                return ServiceResponse<decimal>.SuccessResponse(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<int>> GetPaymentsCountByStatusAsync(PaymentStatus status)
        {
            try
            {
                var count = await _context.Payments.CountAsync(p => p.Status == status);
                return ServiceResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments count by status");
                return ServiceResponse<int>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<decimal>> GetPaymentsByOrderAsync(Guid orderId)
        {
            try
            {
                var total = await _context.Payments
                    .Where(p => p.OrderId == orderId && p.Status == PaymentStatus.PaidInFull)
                    .SumAsync(p => p.Amount);

                return ServiceResponse<decimal>.SuccessResponse(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments by order");
                return ServiceResponse<decimal>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<ServiceResponse<List<PaymentDto>>> GetPaymentsByOrderAsync(Guid orderId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await GetAllPaymentsAsync(
                    orderId: orderId,
                    pageNumber: pageNumber,
                    pageSize: pageSize);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments by order");
                return ServiceResponse<List<PaymentDto>>.ErrorResponse("Đã xảy ra lỗi");
            }
        }
    }
}
