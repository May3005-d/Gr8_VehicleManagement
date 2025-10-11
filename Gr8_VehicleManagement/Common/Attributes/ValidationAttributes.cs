using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Gr8_VehicleManagement.Common.Attributes
{
    /// <summary>
    /// Custom validation attribute for Vietnamese phone numbers
    /// </summary>
    public class VietnamesePhoneAttribute : ValidationAttribute
    {
        private static readonly Regex PhoneRegex = new Regex(@"^(\+84|84|0)[1-9][0-9]{8,9}$", RegexOptions.Compiled);

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            var phone = value.ToString();
            if (string.IsNullOrEmpty(phone)) return true;
            
            return PhoneRegex.IsMatch(phone);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} phải là số điện thoại Việt Nam hợp lệ (VD: 0123456789, +84123456789)";
        }
    }

    /// <summary>
    /// Custom validation attribute for Vietnamese ID numbers
    /// </summary>
    public class VietnameseIdAttribute : ValidationAttribute
    {
        private static readonly Regex IdRegex = new Regex(@"^[0-9]{9,12}$", RegexOptions.Compiled);

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            var id = value.ToString();
            if (string.IsNullOrEmpty(id)) return true;
            
            return IdRegex.IsMatch(id);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} phải là số CMND/CCCD hợp lệ (9-12 chữ số)";
        }
    }

    /// <summary>
    /// Custom validation attribute for Vietnamese email addresses
    /// </summary>
    public class VietnameseEmailAttribute : ValidationAttribute
    {
        private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            var email = value.ToString();
            if (string.IsNullOrEmpty(email)) return true;
            
            return EmailRegex.IsMatch(email);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} phải là địa chỉ email hợp lệ";
        }
    }

    /// <summary>
    /// Custom validation attribute for Vietnamese names (no special characters)
    /// </summary>
    public class VietnameseNameAttribute : ValidationAttribute
    {
        private static readonly Regex NameRegex = new Regex(@"^[a-zA-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂÂÊÔƯăâêôư\s]+$", RegexOptions.Compiled);

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            var name = value.ToString();
            if (string.IsNullOrEmpty(name)) return true;
            
            return NameRegex.IsMatch(name);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} chỉ được chứa chữ cái và khoảng trắng";
        }
    }

    /// <summary>
    /// Custom validation attribute for positive decimal values
    /// </summary>
    public class PositiveDecimalAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            if (value is decimal decimalValue)
            {
                return decimalValue > 0;
            }
            
            if (decimal.TryParse(value.ToString(), out decimal parsedValue))
            {
                return parsedValue > 0;
            }
            
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} phải là số dương";
        }
    }

    /// <summary>
    /// Custom validation attribute for future dates
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            if (value is DateTime dateValue)
            {
                return dateValue > DateTime.Now;
            }
            
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} phải là ngày trong tương lai";
        }
    }

    /// <summary>
    /// Custom validation attribute for past dates
    /// </summary>
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            if (value is DateTime dateValue)
            {
                return dateValue < DateTime.Now;
            }
            
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} phải là ngày trong quá khứ";
        }
    }

    /// <summary>
    /// Custom validation attribute for file size
    /// </summary>
    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly long _maxSizeInBytes;

        public FileSizeAttribute(long maxSizeInMB)
        {
            _maxSizeInBytes = maxSizeInMB * 1024 * 1024;
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            if (value is Microsoft.AspNetCore.Http.IFormFile file)
            {
                return file.Length <= _maxSizeInBytes;
            }
            
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            var maxSizeInMB = _maxSizeInBytes / (1024 * 1024);
            return $"{name} không được vượt quá {maxSizeInMB}MB";
        }
    }

    /// <summary>
    /// Custom validation attribute for file extensions
    /// </summary>
    public class FileExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;

        public FileExtensionAttribute(params string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // Let Required attribute handle null values
            
            if (value is Microsoft.AspNetCore.Http.IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return _allowedExtensions.Contains(extension);
            }
            
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            var extensions = string.Join(", ", _allowedExtensions);
            return $"{name} chỉ được phép các định dạng: {extensions}";
        }
    }
}
