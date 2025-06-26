using InvoiceManagement.DataAccess;
using InvoiceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceManagement.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        public List<InvoiceHeader> Invoices { get; set; }
        public string SearchTerm { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public void OnGet(string searchTerm, int page = 1)
        {
            SearchTerm = searchTerm;
            CurrentPage = page;
            string sortCol = "InvoiceDate";
            string sortDir = "DESC";
            InvoiceDAL dal = new();
            Invoices = dal.GetInvoices(SearchTerm, CurrentPage, PageSize,sortCol,sortDir);
        }

        public JsonResult OnGetGetAllInvoices()
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
            var invoices = dal.GetInvoices(search, pageNumber, length, sortColumn, sortDir);
            var totalCount = dal.GetInvoiceCount(search);

            return new JsonResult(new
            {
                recordsTotal = totalCount,
                recordsFiltered = totalCount,
                data = invoices
            });
        }

    }
}

