using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Models;

namespace IdentityApp.Authorization
{
    public class InvoiceCreatorAuthorizationHandler : 
        AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        UserManager<IdentityUser> _userManager;
        public InvoiceCreatorAuthorizationHandler(UserManager<IdentityUser> userManager)
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

            // this handler is meant for CRUD operations only
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            // this checks to make sure the user is the one who created the invoice before allowing this to continue
            if (invoice.CreatorId == _userManager.GetUserId(context.User))
                context.Succeed(requirement);

            // all else has failed
            return Task.CompletedTask;
        }
    }
}
