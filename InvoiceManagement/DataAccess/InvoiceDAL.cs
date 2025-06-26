using InvoiceManagement.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceManagement.DataAccess
{
    public class InvoiceDAL
    {
        public List<InvoiceHeader> GetInvoices(string searchTerm, int pageNumber, int pageSize, string sortColumn, string sortDir)
        {
            List<InvoiceHeader> invoices = new();

            using SqlConnection conn = new(ConnectionHelper.Get());
            using SqlCommand cmd = new("usp_GetInvoices", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? "");
            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@SortColumn", sortColumn);
            cmd.Parameters.AddWithValue("@SortDirection", sortDir);
            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                invoices.Add(new InvoiceHeader
                {
                    InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                    Title = reader["Title"].ToString(),
                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                    TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount"))
                        ? 0  
                        : Convert.ToDecimal(reader["TotalAmount"])
                });
            }

            return invoices;
        }

        public List<InvoiceLineItem> GetInvoiceLineItems(int invoiceId, string searchTerm, int pageNumber, int pageSize, string sortColumn, string sortDirection)
        {
            List<InvoiceLineItem> items = new();
            using SqlConnection conn = new(ConnectionHelper.Get());
            using SqlCommand cmd = new("usp_GetInvoiceLineItems", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
            cmd.Parameters.AddWithValue("@SearchTerm", searchTerm ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            cmd.Parameters.AddWithValue("@SortColumn", sortColumn ?? "ItemCode");
            cmd.Parameters.AddWithValue("@SortDirection", sortDirection ?? "ASC");

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new InvoiceLineItem
                {
                    ItemCode = reader["ItemCode"].ToString(),
                    Description = reader["Description"].ToString(),
                    UnitRate = Convert.ToDecimal(reader["UnitRate"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                });
            }

            return items;
        }

        public List<InvoiceLineItem> GetLineItemsByInvoiceId(int invoiceId)
        {
            List<InvoiceLineItem> items = new();

            using SqlConnection conn = new(ConnectionHelper.Get());
            using SqlCommand cmd = new("GetInvoiceLineItemsByInvoiceId", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new InvoiceLineItem
                {
                    LineItemID = Convert.ToInt32(reader["LineItemID"]),
                    InvoiceID = Convert.ToInt32(reader["InvoiceID"]),
                    ItemCode = reader["ItemCode"].ToString(),
                    Description = reader["Description"].ToString(),
                    UnitRate = Convert.ToDecimal(reader["UnitRate"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                });
            }

            return items;
        }


        public int GetInvoiceCount(string search)
        {
            using SqlConnection conn = new(ConnectionHelper.Get());
            using SqlCommand cmd = new("GetInvoiceCount", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@SearchTerm", search ?? "");

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public InvoiceHeader GetInvoiceById(int id)
        {
            using SqlConnection conn = new SqlConnection(ConnectionHelper.Get());
            using SqlCommand cmd = new SqlCommand("GetInvoiceById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@InvoiceID", id);

            conn.Open();
            using SqlDataReader reader = cmd.ExecuteReader();

            InvoiceHeader invoice = null;

            if (reader.Read())
            {
                invoice = new InvoiceHeader
                {
                    InvoiceID = reader.GetInt32(0),
                    InvoiceNumber = reader.GetString(1),
                    Title = reader.GetString(2),
                    InvoiceDate = reader.GetDateTime(3),
                    LineItems = new List<InvoiceLineItem>()
                };
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    invoice.LineItems.Add(new InvoiceLineItem
                    {
                        LineItemID = reader.GetInt32(0),
                        InvoiceID = reader.GetInt32(1),
                        ItemCode = reader.GetString(2),
                        Description = reader.GetString(3),
                        UnitRate = reader.GetDecimal(4),
                        Quantity = reader.GetInt32(5)
                        
                    });
                }
            }

            return invoice;
        }


        public void AddInvoiceWithItems(InvoiceHeader invoice)
        {
            using SqlConnection conn = new(ConnectionHelper.Get());
            using SqlCommand cmd = new("AddInvoiceWithItems", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
            cmd.Parameters.AddWithValue("@Title", invoice.Title);
            cmd.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);

            SqlParameter lineItemsParam = cmd.Parameters.AddWithValue("@LineItems", CreateLineItemsDataTable(invoice.LineItems));
            lineItemsParam.SqlDbType = SqlDbType.Structured;
            lineItemsParam.TypeName = "InvoiceLineItemType";

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void UpdateInvoiceWithItems(InvoiceHeader invoice, List<int> deletedItemIds)
        {
            using SqlConnection conn = new(ConnectionHelper.Get());
            using SqlCommand cmd = new("UpdateInvoiceWithItems", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@InvoiceID", invoice.InvoiceID);
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoice.InvoiceNumber);
            cmd.Parameters.AddWithValue("@Title", invoice.Title);
            cmd.Parameters.AddWithValue("@InvoiceDate", invoice.InvoiceDate);

            SqlParameter lineItemsParam = cmd.Parameters.AddWithValue("@LineItems", CreateLineItemsDataTable(invoice.LineItems));
            lineItemsParam.SqlDbType = SqlDbType.Structured;
            lineItemsParam.TypeName = "InvoiceLineItemType";

            SqlParameter deletedItemsParam = cmd.Parameters.AddWithValue("@DeletedLineItems", CreateDeletedItemsTable(deletedItemIds));
            deletedItemsParam.SqlDbType = SqlDbType.Structured;
            deletedItemsParam.TypeName = "DeletedLineItemType";


            conn.Open();
            cmd.ExecuteNonQuery();
        }


        private DataTable CreateLineItemsDataTable(List<InvoiceLineItem> lineItems)
        {
            DataTable table = new();
            table.Columns.Add("LineItemID", typeof(int));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("UnitRate", typeof(decimal));
            table.Columns.Add("Quantity", typeof(int));

            foreach (var item in lineItems)
            {
                table.Rows.Add(item.LineItemID, item.ItemCode, item.Description, item.UnitRate, item.Quantity);
            }

            return table;
        }

        private DataTable CreateDeletedItemsTable(List<int> deletedItemIds)
        {
            DataTable table = new();
            table.Columns.Add("LineItemID", typeof(int));

            foreach (var id in deletedItemIds)
            {
                table.Rows.Add(id);
            }

            return table;
        }


    }
}
