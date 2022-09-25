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
    public class DetailsModel : DI_BasePageModel
    {

        public DetailsModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager) { }

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

            // can the user view it?
            var isCreator = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Read);

            var isManager = User.IsInRole(Constants.InvoiceManagerRole);

            if (isCreator.Succeeded == false && isManager == false)
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, InvoiceStatus status)
        {

            var invoice = await Context.Invoice.AsNoTracking().FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }

            // which operations authorization does the user require
            var invoiceOperation = (status == InvoiceStatus.Approved)
                ? InvoiceOperations.Approve : InvoiceOperations.Reject;

            // can the user update it?
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, invoice, invoiceOperation);

            if (isAuthorized.Succeeded == false)
                return Forbid();

            invoice.Status = status;
            Context.Invoice.Update(invoice);
            //Context.Attach(Invoice).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(Invoice.InvoiceID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool InvoiceExists(int id)
        {
            return (Context.Invoice?.Any(e => e.InvoiceID == id)).GetValueOrDefault();
        }

    }
}
