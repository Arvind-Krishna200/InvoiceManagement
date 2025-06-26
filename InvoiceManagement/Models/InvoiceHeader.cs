using InvoiceManagement.Models;
using System.ComponentModel.DataAnnotations;

public class InvoiceHeader
{
    public int InvoiceID { get; set; }

   
    public string InvoiceNumber { get; set; }

    
    public string Title { get; set; }


    public DateTime InvoiceDate { get; set; }


    public decimal TotalAmount { get; set; } 


    public List<InvoiceLineItem> LineItems { get; set; } = new();

}
