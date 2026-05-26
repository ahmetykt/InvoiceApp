using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public string? Service { get; set; } = "";

        [Precision(16, 2)]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Bire-Çok İlişki Bağlantısı (Foreign Key)
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}