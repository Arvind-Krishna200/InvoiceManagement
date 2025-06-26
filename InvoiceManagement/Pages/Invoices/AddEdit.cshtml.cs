using InvoiceManagement.DataAccess;
using InvoiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace InvoiceManagement.Pages.Invoices
{
    public class AddEditModel : PageModel
    {
        [BindProperty]
        public InvoiceHeader Invoice { get; set; }
        public string DeletedLineItemIDs { get; set; }
        public IActionResult OnGet(int? id)
        {
            if (id.HasValue)
            {
                InvoiceDAL dal = new();
                Invoice = dal.GetInvoiceById(id.Value);

                if (Invoice == null)
                {
                    return NotFound();
                }
            }
            else
            {
                Invoice = new InvoiceHeader
                {
                    InvoiceDate = DateTime.Now,
                    LineItems = new List<InvoiceLineItem>()
                };
            }

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            InvoiceDAL dal = new();

            if (Invoice.InvoiceID > 0)
            {
                var deletedIds = string.IsNullOrEmpty(DeletedLineItemIDs)
                   ? new List<int>()
                   : JsonSerializer.Deserialize<List<int>>(DeletedLineItemIDs);

                dal.UpdateInvoiceWithItems(Invoice,deletedIds);
            }  
            else
                dal.AddInvoiceWithItems(Invoice);

            return RedirectToPage("Index");
        }
        public JsonResult OnGetGetLineItems(int id)
        {
            var request = Request.Query;

            string search = request["search[value]"];
            int start = int.Parse(request["start"]);
            int length = int.Parse(request["length"]);
            int pageNumber = (start / length) + 1;

            string sortColumnIndex = request["order[0][column]"];
            string sortColumn = request[$"columns[{sortColumnIndex}][data]"];
            string sortDir = request["order[0][dir]"];

            InvoiceDAL dal = new();
            var items = dal.GetInvoiceLineItems(id, search, pageNumber, length, sortColumn, sortDir);
            var totalCount = items.Count; // or write a separate count SP if you want accurate paging

            return new JsonResult(new
            {
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = items
            });
        }

        public JsonResult OnGetLineItemsPage(int invoiceId, int pageNumber, int pageSize)
        {
            InvoiceDAL dal = new();
            var allItems = dal.GetLineItemsByInvoiceId(invoiceId);

            var pagedItems = allItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new JsonResult(new
            {
                total = allItems.Count,
                data = pagedItems
            });
        }

    }
}
