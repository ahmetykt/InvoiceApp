using InvoiceApp.Models;
using InvoiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace InvoiceApp.Pages.Invoices
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InvoiceDto InvoiceDto { get; set; } = new();

        public void OnGet()
        {
            // Sayfa ilk açıldığında faturanın tamamen boş görünmemesi için
            // kullanıcıya varsayılan olarak 1 tane boş hizmet satırı gönderiyoruz.
            InvoiceDto.Items.Add(new InvoiceItemDto());
        }

        public IActionResult OnPost()
        {
            // Model kuralları (zorunlu alanlar, formatlar vb.) sağlanmadıysa aynı sayfada kal.
            if (!ModelState.IsValid) return Page();

            // DTO (Data Transfer Object) üzerinden aldığımız güvenli verileri,
            // veritabanı modelimiz olan 'Invoice' nesnesine aktarıyoruz (Mapping işlemi).
            Invoice invoice = new()
            {
                Number = InvoiceDto.Number,
                Status = InvoiceDto.Status,
                IssueDate = InvoiceDto.IssueDate,
                DueDate = InvoiceDto.DueDate,
                ClientName = InvoiceDto.ClientName,
                Email = InvoiceDto.Email,
                Phone = InvoiceDto.Phone,
                Address = InvoiceDto.Address,

                // Formdan dinamik olarak gelen birden fazla hizmet satırını listeye dönüştürüp ana faturaya bağlıyoruz.
                Items = InvoiceDto.Items.Select(i => new InvoiceItem
                {
                    Service = i.Service,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Faturayı ve içindeki kalemleri tek seferde veritabanına ekle ve kaydet.
            _context.Invoices.Add(invoice);
            _context.SaveChanges();

            // İşlem başarılı olduktan sonra kullanıcıyı faturalar listesine yönlendir.
            return RedirectToPage("/Invoices/Index");
        }
    }
}