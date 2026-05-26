using InvoiceApp.Models;
using InvoiceApp.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvoiceApp.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext context;

        // Fatura listesini arayüze (HTML'e) taşımak için oluşturduğumuz liste
        public List<Invoice> invoiceList { get; set; } = new();

        // Sayfalama (Pagination) işlemleri için gerekli değişkenler
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;

        // Önceki veya sonraki sayfaya geçiş butonlarının aktif/pasif durumunu kontrol eder
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Dependency Injection (Bağımlılık Enjeksiyonu) ile veritabanı bağlantımızı alıyoruz
        public IndexModel(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void OnGet(int? pageIndex, int? pageSize)
        {
            // Kullanıcıdan sayfa boyutu veya indeksi gelmezse varsayılan değerleri (1. sayfa, 10 kayıt) atıyoruz
            PageSize = pageSize ?? 10;
            PageIndex = pageIndex ?? 1;

            // Veritabanındaki toplam fatura sayısını bulup, sayfa boyutuna bölerek 'Toplam Sayfa Sayısını' hesaplıyoruz
            int totalItems = context.Invoices.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Kullanıcının geçersiz sayfa numarası (örn: -1 veya 1000) girmesini engelliyoruz
            if (PageIndex < 1) PageIndex = 1;
            if (TotalPages > 0 && PageIndex > TotalPages) PageIndex = TotalPages;

            // Entity Framework Eager Loading (.Include) kullanımı :
            // Faturanın alt kalemlerini de veritabanından çekiyoruz ki arayüzde toplam tutar (Quantity * UnitPrice) hesaplanabilsin.
            invoiceList = context.Invoices
                .Include(i => i.Items)               // Alt tabloyu sorguya dahil et
                .OrderByDescending(i => i.IssueDate) // En yeni faturalar en üstte listelensin
                .Skip((PageIndex - 1) * PageSize)    // Önceki sayfaların kayıtlarını atla (Pagination mantığı)
                .Take(PageSize)                      // Sadece bu sayfa için belirlenen limit kadar kayıt al
                .ToList();
        }
    }
}