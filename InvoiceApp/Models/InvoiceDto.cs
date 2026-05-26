using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Models
{
    // Her bir fatura satırını temsil edecek yeni taşıyıcı sınıfımız
    public class InvoiceItemDto
    {
        [Required]
        public string? Service { get; set; } = "";

        [Range(1, 999999, ErrorMessage = "Unit price is not valid")]
        public decimal UnitPrice { get; set; }

        [Range(1, 99, ErrorMessage = "Quantity is not valid")]
        public int Quantity { get; set; }
    }

    public class InvoiceDto
    {
        [Required]
        public string? Number { get; set; } = "";

        [Required]
        public string? Status { get; set; } = "";

        public DateOnly? IssueDate { get; set; }
        public DateOnly? DueDate { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        public string? ClientName { get; set; } = "";

        [Required, EmailAddress]
        public string? Email { get; set; } = "";

        [Phone]
        public string Phone { get; set; } = "";

        public string? Address { get; set; } = "";

        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}