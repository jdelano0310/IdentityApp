using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    // even though there is a rule that authentication is required via fallback, this tag excludes this page from
    // requiring an authorized user to access it
    [AllowAnonymous]   
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager) { }

        public IList<Invoice> Invoice { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (Context.Invoice != null)
            {
                // change the ToListArray to return only those invoices the logged in user created
                //Invoice = await _context.Invoice.ToListAsync();

                // now that we have a manager role that can see all invoices, get them all from the database
                // then determine if the user is a manager, if not then filter the invoice list on their userid

                var invoices = from inv in Context.Invoice select inv;

                var isAdmin = User.IsInRole(Constants.InvoiceAdministratorRole);

                if (!isAdmin)
                {
                    var isManager = User.IsInRole(Constants.InvoiceManagerRole);

                    if (!isManager)
                    {
                        var userId = UserManager.GetUserId(User);
                        invoices = invoices.Where(inv => inv.CreatorId == userId);
                    }
                }

                Invoice = await invoices.ToListAsync();

                //Invoice = await Context.Invoice
                //    .Where(inv => inv.CreatorId == userId)
                //    .ToListAsync();
            }
        }
    }
}
