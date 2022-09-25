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
    public class DeleteModel : DI_BasePageModel
    {

        public DeleteModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager) { }

        [BindProperty]
      public Invoice Invoice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            var invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceID == id);

            if (invoice == null)
            {
                return NotFound();
            }
            else 
            {
                Invoice = invoice;
            }

            // can the user delete it?
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Delete);

            if (isAuthorized.Succeeded == false)
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            var invoice = await Context.Invoice.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }
            Invoice = invoice;

            // can the user delete it?
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Delete);

            if (isAuthorized.Succeeded == false)
                return Forbid();

            Context.Invoice.Remove(Invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
