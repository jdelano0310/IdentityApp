using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Models;

namespace IdentityApp.Authorization
{
    public class InvoiceAdministratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        UserManager<IdentityUser> _userManager;

        public InvoiceAdministratorAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Invoice invoice)
        {
            // if there isn't a user or an invoice then return without authorizing anything
            if (context.User == null || invoice == null)
                return Task.CompletedTask;

            // this checks to make sure the user is an administrator
            if (context.User.IsInRole(Constants.InvoiceAdministratorRole))
                context.Succeed(requirement);

            // all else has failed
            return Task.CompletedTask;
        }
    }
}
