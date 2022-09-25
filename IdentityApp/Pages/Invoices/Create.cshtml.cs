using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    // change the base class of the page from the standard PageModel to our DI_BasePageModel
    //public class CreateModel : PageModel
    public class CreateModel : DI_BasePageModel
    {
        // this isn't needed because the DI_BasePageModel has it
        //private readonly IdentityApp.Data.ApplicationDbContext _context;

        // update the CreateModel to match that of our DI_BasePageModel
        //public CreateModel(IdentityApp.Data.ApplicationDbContext context)
        public CreateModel(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager)
        {
            // this isn't needed because the DI_BasePageModel has it
            //_context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Invoice Invoice { get; set; } = default!;

        // change this task to utilize the Context found in the DI_BasePageModel since 
        // it is providing the context to the DB
        //
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        //public async Task<IActionResult> OnPostAsync()
        //{
        //  if (!ModelState.IsValid || _context.Invoice == null || Invoice == null)
        //    {
        //        return Page();
        //    }

        //    _context.Invoice.Add(Invoice);
        //    await _context.SaveChangesAsync();

        //    return RedirectToPage("./Index");
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            // no need to do this check now that the page is based on the DI_BasePageModel 
            //if (!ModelState.IsValid || Context.Invoice == null || Invoice == null)
            //{
            //    return Page();
            //}

            Invoice.CreatorId = UserManager.GetUserId(User);

            // check for the authorized rules are satisfied
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Create);

            if (isAuthorized.Succeeded == false)
                return Forbid();

            Context.Invoice.Add(Invoice);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
