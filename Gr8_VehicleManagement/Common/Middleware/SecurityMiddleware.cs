using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Gr8_VehicleManagement.Common.Middleware
{
    /// <summary>
    /// Security middleware for XSS prevention, CSRF protection, and security headers
    /// </summary>
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityMiddleware> _logger;

        public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Add security headers
                AddSecurityHeaders(context);

                // XSS prevention
                await PreventXSS(context);

                // CSRF protection
                await ProtectCSRF(context);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Security middleware error");
                throw;
            }
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            var response = context.Response;

            // Prevent XSS attacks
            response.Headers["X-Content-Type-Options"] = "nosniff";
            response.Headers["X-Frame-Options"] = "DENY";
            response.Headers["X-XSS-Protection"] = "1; mode=block";
            
            // Content Security Policy
            response.Headers["Content-Security-Policy"] = 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' https://cdn.jsdelivr.net; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none';";

            // Referrer Policy
            response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Permissions Policy
            response.Headers["Permissions-Policy"] = 
                "geolocation=(), microphone=(), camera=(), payment=(), usb=(), magnetometer=(), gyroscope=(), accelerometer=()";
        }

        private async Task PreventXSS(HttpContext context)
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                // Check for potential XSS in form data
                var form = await context.Request.ReadFormAsync();
                
                foreach (var field in form)
                {
                    var value = field.Value.ToString();
                    if (ContainsXSS(value))
                    {
                        _logger.LogWarning("Potential XSS attack detected from IP: {IP}, Field: {Field}, Value: {Value}", 
                            context.Connection.RemoteIpAddress, field.Key, value);
                        
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid input detected");
                        return;
                    }
                }
            }
        }

        private bool ContainsXSS(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var xssPatterns = new[]
            {
                "<script",
                "javascript:",
                "onload=",
                "onerror=",
                "onclick=",
                "onmouseover=",
                "onfocus=",
                "onblur=",
                "onchange=",
                "onsubmit=",
                "onreset=",
                "onselect=",
                "onkeydown=",
                "onkeyup=",
                "onkeypress=",
                "onmousedown=",
                "onmouseup=",
                "onmousemove=",
                "onmouseout=",
                "onmouseover=",
                "onmouseenter=",
                "onmouseleave=",
                "oncontextmenu=",
                "ondblclick=",
                "onabort=",
                "oncanplay=",
                "oncanplaythrough=",
                "ondurationchange=",
                "onemptied=",
                "onended=",
                "onerror=",
                "onloadeddata=",
                "onloadedmetadata=",
                "onloadstart=",
                "onpause=",
                "onplay=",
                "onplaying=",
                "onprogress=",
                "onratechange=",
                "onseeked=",
                "onseeking=",
                "onstalled=",
                "onsuspend=",
                "ontimeupdate=",
                "onvolumechange=",
                "onwaiting=",
                "expression(",
                "url(",
                "vbscript:",
                "data:",
                "&#",
                "&lt;script",
                "&lt;/script",
                "&lt;iframe",
                "&lt;/iframe",
                "&lt;object",
                "&lt;/object",
                "&lt;embed",
                "&lt;/embed",
                "&lt;link",
                "&lt;/link",
                "&lt;meta",
                "&lt;/meta",
                "&lt;style",
                "&lt;/style"
            };

            var lowerInput = input.ToLowerInvariant();
            return xssPatterns.Any(pattern => lowerInput.Contains(pattern));
        }

        private async Task ProtectCSRF(HttpContext context)
        {
            // CSRF protection for state-changing operations
            if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE")
            {
                // Skip CSRF validation for authentication pages in development
                if (context.Request.Path.StartsWithSegments("/Account/Register") ||
                    context.Request.Path.StartsWithSegments("/Account/Login") ||
                    context.Request.Path.StartsWithSegments("/Account/ForgotPassword") ||
                    context.Request.Path.StartsWithSegments("/Account/Logout"))
                {
                    _logger.LogInformation("Skipping CSRF validation for authentication pages");
                    return;
                }

                // Check for CSRF token in form data
                var form = await context.Request.ReadFormAsync();
                var csrfToken = form["__RequestVerificationToken"].FirstOrDefault();
                
                if (string.IsNullOrEmpty(csrfToken))
                {
                    _logger.LogWarning("CSRF token missing from request from IP: {IP}", 
                        context.Connection.RemoteIpAddress);
                    
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("CSRF token missing");
                    return;
                }

                // Validate CSRF token (simplified - in production, use proper CSRF validation)
                if (!IsValidCSRFToken(csrfToken, context))
                {
                    _logger.LogWarning("Invalid CSRF token from IP: {IP}", 
                        context.Connection.RemoteIpAddress);
                    
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid CSRF token");
                    return;
                }
            }
        }

        private bool IsValidCSRFToken(string token, HttpContext context)
        {
            // Simplified CSRF validation - in production, use proper CSRF validation
            // This is a basic implementation for demonstration
            var sessionToken = context.Session.GetString("CSRFToken");
            return !string.IsNullOrEmpty(sessionToken) && sessionToken == token;
        }
    }

    /// <summary>
    /// Extension method to register security middleware
    /// </summary>
    public static class SecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityMiddleware>();
        }
    }
}
