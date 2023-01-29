using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MCBA_Library;

namespace Assignment2.Filter;

public class AuthorizeCustomerAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {

        var customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if (!customerID.HasValue)
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
