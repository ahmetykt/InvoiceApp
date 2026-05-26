using InvoiceApp.Models;
using InvoiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InvoiceApp.Pages.Invoices
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection ile veritabanı bağlamını (context) projeye dahil ediyoruz
        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sayfadaki form ile eşleşecek DTO (Data Transfer Object) nesnemiz
        [BindProperty]
        public InvoiceDto InvoiceDto { get; set; } = new();

        public Invoice Invoice { get; set; } = new();

        // Sayfanın hangi modda açıldığını URL'den (QueryString) alıyoruz 
        [BindProperty(SupportsGet = true)]
        public string Mode { get; set; } = "details";

        public string successMessage = "";

        public IActionResult OnGet(int id, string? mode)
        {
            Mode = string.IsNullOrEmpty(mode) ? "details" : mode.ToLower();

            // Veritabanından faturayı ve Entity Framework 'Include' ile ilişkili alt kalemlerini (Items) getiriyoruz
            var invoice = _context.Invoices.Include(i => i.Items).FirstOrDefault(i => i.Id == id);

            if (invoice == null) return RedirectToPage("/Invoices/Index");

            Invoice = invoice;

            // Veritabanından gelen verileri formda göstermek için güvenli nesnemiz olan DTO'ya aktarıyoruz (Mapping)
            InvoiceDto = new InvoiceDto
            {
                Number = Invoice.Number,
                Status = Invoice.Status,
                IssueDate = Invoice.IssueDate,
                DueDate = Invoice.DueDate,
                ClientName = Invoice.ClientName,
                Email = Invoice.Email,
                Phone = Invoice.Phone,
                Address = Invoice.Address,
                Items = Invoice.Items.Select(i => new InvoiceItemDto
                {
                    Service = i.Service,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Page();
        }

        public IActionResult OnPost(int id, string? mode)
        {
            Mode = string.IsNullOrEmpty(mode) ? "details" : mode.ToLower();

            // Güvenlik ve veri bütünlüğü için güncellenecek/silinecek faturayı tekrar buluyoruz
            var invoice = _context.Invoices.Include(i => i.Items).FirstOrDefault(i => i.Id == id);

            if (invoice == null) return RedirectToPage("/Invoices/Index");

            Invoice = invoice;

            // Eğer kullanıcı Delete modundan gelip onayladıysa faturayı tamamen kaldırıyoruz
            if (Mode == "delete")
            {
                _context.Invoices.Remove(invoice);
                _context.SaveChanges();

                // İşlem sonrası ana sayfada gösterilecek başarı mesajını TempData'ya yüklüyoruz
                TempData["SuccessMessage"] = "Fatura veri tabanından başarıyla silindi.";
                return RedirectToPage("/Invoices/Index");
            }

            // Eğer kullanıcı Edit modundaysa form verilerini güncelliyoruz
            if (Mode == "edit")
            {
                // Formdaki zorunlu alanlar doldurulmamışsa veya format hatalıysa işlemi durdur
                if (!ModelState.IsValid) return Page();

                // 1. Ana fatura üst bilgilerini güncelle
                invoice.Number = InvoiceDto.Number;
                invoice.Status = InvoiceDto.Status;
                invoice.IssueDate = InvoiceDto.IssueDate;
                invoice.DueDate = InvoiceDto.DueDate;
                invoice.ClientName = InvoiceDto.ClientName;
                invoice.Email = InvoiceDto.Email;
                invoice.Phone = InvoiceDto.Phone;
                invoice.Address = InvoiceDto.Address;

                // 2. Çoklu satır mantığı: İlişkili eski kalemleri veri tabanından tamamen temizle
                _context.InvoiceItems.RemoveRange(invoice.Items);

                // 3. Formdan gelen güncel satır listesini faturaya yeni kalemler olarak ekle
                if (InvoiceDto.Items != null)
                {
                    invoice.Items = InvoiceDto.Items.Select(i => new InvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        Service = i.Service,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity
                    }).ToList();
                }

                _context.SaveChanges();

                // Güncelleme işlemi bittikten sonra başarı mesajını hazırla ve listeye dön
                TempData["SuccessMessage"] = "Fatura başarıyla güncellendi.";

                return RedirectToPage("/Invoices/Index");
            }

            return Page();
        }
    }
}