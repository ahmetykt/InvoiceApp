using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string? Number { get; set; } = "";
        public string? Status { get; set; } = "";
        public DateOnly? IssueDate { get; set; }
        public DateOnly? DueDate { get; set; }

        // İstemci ayrıntıları
        public string? ClientName { get; set; } = "";
        public string? Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string? Address { get; set; } = "";

        // Bire-Çok (One-to-Many) İlişkisi: 
        // Bir faturanın birden çok kalemi (satırı) olur.
        public List<InvoiceItem> Items { get; set; } = new();
    }
}