using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Models;

namespace IdentityApp.Authorization
{
    public class InvoiceManagerAuthorizationHandler 
        : AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        UserManager<IdentityUser> _userManager;

        public InvoiceManagerAuthorizationHandler(UserManager<IdentityUser> userManager)
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

            // this handler is checking for the ability to approve or rejecting invoices only
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName)
            {
                return Task.CompletedTask;
            }

            // this checks to make sure the user is an invoice manager
            if (context.User.IsInRole(Constants.InvoiceManagerRole))
                context.Succeed(requirement);

            // all else has failed
            return Task.CompletedTask;
        }
    }
}
