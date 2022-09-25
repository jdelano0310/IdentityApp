using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class EditModel : DI_BasePageModel
    {

        public EditModel(
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

            var invoice =  await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }
            Invoice = invoice;

            // can the user update it?
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Update);

            if (isAuthorized.Succeeded == false || Invoice.Status != InvoiceStatus.Submitted)
                return Forbid();

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            //if (!ModelState.IsValid)     the form doesn't have the creator id (right now) so it can never be valid - 
            //{                            comment this out for now
            //    return Page();
            //}

            // to resolve the error that something is already being tracked (like the record is locked?) add the
            // AsNoTracking to the search
            //var invoice = await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceID == id);
            var invoice = await Context.Invoice.AsNoTracking().FirstOrDefaultAsync(m => m.InvoiceID == id);
            if (invoice == null)
            {
                return NotFound();
            }
            
            Invoice.CreatorId = invoice.CreatorId;

            // can the user update it?
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Update);

            if (isAuthorized.Succeeded == false)
                return Forbid();

            Context.Attach(Invoice).State = EntityState.Modified;

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
