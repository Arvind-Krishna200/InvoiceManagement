using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.Models
{
    public class InvoiceLineItem
    {
        public int LineItemID { get; set; }
        public int InvoiceID { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal UnitRate { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitRate * Quantity;
    }
}
