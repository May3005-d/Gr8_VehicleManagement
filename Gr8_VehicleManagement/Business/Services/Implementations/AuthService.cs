using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Find user by username
                var user = await _context.Users
                    .Include(u => u.Dealer)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng");
                }

                // Plain text password comparison for development
                bool isValidPassword = loginDto.Password == user.Password;

                if (!isValidPassword)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng");
                }
                await _context.SaveChangesAsync();

                // Map to DTO
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    UserType = user.UserType,
                    DealerId = user.DealerId,
                    DealerName = user.Dealer?.Name ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };

                _logger.LogInformation("User {UserId} logged in successfully", user.Id);

                return ServiceResponse<UserDto>.SuccessResponse(userDto, "Đăng nhập thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return ServiceResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi đăng nhập");
            }
        }

        public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Dealer)
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Không tìm thấy người dùng");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FullName = user.FullName,
                    UserType = user.UserType,
                    DealerId = user.DealerId,
                    DealerName = user.Dealer?.Name ?? string.Empty,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
                };

                return ServiceResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID");
                return ServiceResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi");
            }
        }

        public async Task<bool> UpdateLastLoginAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login");
                return false;
            }
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null) return false;

                // Plain text password comparison for development
                return password == user.Password;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user");
                return false;
            }
        }
    }
}

