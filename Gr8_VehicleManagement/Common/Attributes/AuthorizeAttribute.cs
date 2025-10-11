using Gr8_VehicleManagement.Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gr8_VehicleManagement.Common.Attributes
{
    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            
            if (!session.IsAuthenticated())
            {
                context.Result = new RedirectToPageResult("/Account/Login", new { returnUrl = context.HttpContext.Request.Path });
            }

            base.OnActionExecuting(context);
        }
    }
}

