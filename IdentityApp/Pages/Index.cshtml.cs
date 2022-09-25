using IdentityApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages
{
    public class IndexModel : PageModel
    {
        public Dictionary<string, int> revenue_submitted;
        public Dictionary<string, int> revenue_approved;
        public Dictionary<string, int> revenue_rejected;

        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            revenue_submitted = new Dictionary<string, int>()
            {
                {"January", 0 },
                {"February", 0 },
                {"March", 0 },
                {"April", 0 },
                {"May", 0 },
                {"June", 0 },
                {"July", 0 },
                {"August", 0 },
                {"September", 0 },
                {"October", 0 },
                {"November", 0 },
                {"December", 0 }
            };

            revenue_approved = new Dictionary<string, int>()
            {
                {"January", 0 },
                {"February", 0 },
                {"March", 0 },
                {"April", 0 },
                {"May", 0 },
                {"June", 0 },
                {"July", 0 },
                {"August", 0 },
                {"September", 0 },
                {"October", 0 },
                {"November", 0 },
                {"December", 0 }
            };

            revenue_rejected = new Dictionary<string, int>()
            {
                {"January", 0 },
                {"February", 0 },
                {"March", 0 },
                {"April", 0 },
                {"May", 0 },
                {"June", 0 },
                {"July", 0 },
                {"August", 0 },
                {"September", 0 },
                {"October", 0 },
                {"November", 0 },
                {"December", 0 }
            };
            var invoices = _context.Invoice.ToList();
            foreach (var invoice in invoices)
            {
                switch (invoice.Status) {
                    case Models.InvoiceStatus.Submitted:
                        revenue_submitted[invoice.InvoiceMonth] += (int)invoice.InvoiceAmount;
                        break;
                    case Models.InvoiceStatus.Approved:
                        revenue_approved[invoice.InvoiceMonth] += (int)invoice.InvoiceAmount;
                        break;
                    case Models.InvoiceStatus.Rejected:
                        revenue_rejected[invoice.InvoiceMonth] += (int)invoice.InvoiceAmount;
                        break;
                }
            }
        }
    }
}