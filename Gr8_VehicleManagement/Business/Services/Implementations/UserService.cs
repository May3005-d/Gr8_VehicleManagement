using Gr8_VehicleManagement.Business.DTOs;
using Gr8_VehicleManagement.Business.Services.Interfaces;
using Gr8_VehicleManagement.Common.Responses;
using Gr8_VehicleManagement.Data;
using Gr8_VehicleManagement.Data.Entities;
using Gr8_VehicleManagement.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Gr8_VehicleManagement.Business.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Không tìm thấy người dùng.");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return ServiceResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                return ServiceResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi tải thông tin người dùng.");
            }
        }

        public async Task<ServiceResponse<UserDto>> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Không tìm thấy người dùng.");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return ServiceResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                return ServiceResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi tải thông tin người dùng.");
            }
        }

        public async Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == dto.Username);

                if (existingUser != null)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Tên đăng nhập đã tồn tại.");
                }

                // Note: Phone number check removed for registration flow
                // since it can conflict with existing admin users

                // Store password as plain text for development
                var hashedPassword = dto.Password;

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = dto.Username,
                    Password = hashedPassword,
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    UserType = dto.UserType,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return ServiceResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", dto.Username);
                return ServiceResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi tạo người dùng.");
            }
        }

        public async Task<ServiceResponse<UserDto>> UpdateUserAsync(UpdateUserDto dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.Id);

                if (user == null)
                {
                    return ServiceResponse<UserDto>.ErrorResponse("Không tìm thấy người dùng.");
                }

                user.FullName = dto.FullName;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return ServiceResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", dto.Id);
                return ServiceResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật người dùng.");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy người dùng.");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xóa người dùng.");
            }
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordDto dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                if (user == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy người dùng.");
                }

                // Kiểm tra mật khẩu hiện tại (plain text comparison for development)
                if (dto.CurrentPassword != user.Password)
                {
                    return ServiceResponse<bool>.ErrorResponse("Mật khẩu hiện tại không đúng.");
                }

                // Cập nhật mật khẩu mới (plain text for development)
                user.Password = dto.NewPassword;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", dto.UserId);
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi đổi mật khẩu.");
            }
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == dto.UserId);

                if (user == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Không tìm thấy người dùng.");
                }

                // Cập nhật mật khẩu mới (plain text for development)
                user.Password = dto.NewPassword;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {UserId}", dto.UserId);
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi đặt lại mật khẩu.");
            }
        }

        public async Task<ServiceResponse<bool>> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return ServiceResponse<bool>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                // Plain text password comparison for development
                if (password != user.Password)
                {
                    return ServiceResponse<bool>.ErrorResponse("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user: {Username}", username);
                return ServiceResponse<bool>.ErrorResponse("Đã xảy ra lỗi khi xác thực người dùng.");
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
