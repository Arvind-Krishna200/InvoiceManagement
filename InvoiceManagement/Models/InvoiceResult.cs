using InvoiceManagement.Models;

namespace InvoiceManagement.Models
{
    public class InvoiceResult
    {
        public List<InvoiceHeader> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
