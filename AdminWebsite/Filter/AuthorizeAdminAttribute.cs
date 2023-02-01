using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminWebsite.Filter
{
    public class AuthorizeAdminAttribute: Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var admin = context.HttpContext.Session.GetInt32("id");
            
            if (!admin.HasValue)
                context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }
}

