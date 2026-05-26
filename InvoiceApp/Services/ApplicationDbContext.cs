using Microsoft.EntityFrameworkCore;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Invoice> Invoices { get; set; } = null!;

        // Yeni Fatura  tablomuzu ekliyoruz
        public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
    }
}
