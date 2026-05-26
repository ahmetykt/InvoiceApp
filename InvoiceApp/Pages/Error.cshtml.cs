using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace InvoiceApp.Pages
{
    // Hata sayfalarının tarayıcı tarafından önbelleğe alınmasını (cache) engelliyoruz.
    // Çünkü kullanıcı her hata aldığında eski bir hatayı değil, anlık olan yeni hatayı görmelidir.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

    // Güvenlik doğrulamasında (CSRF vb.) bir hata çıksa bile bu sayfanın çalışabilmesi için token kontrolünü yoksayıyoruz.
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        // Hatayı takip edebilmek için her isteğe atanan benzersiz kimlik numarası
        public string? RequestId { get; set; }

        // RequestId boş değilse ekranda göstermek için bir kontrol özelliği (Property)
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Sayfa yüklendiğinde çalışacak metot
        public void OnGet()
        {
            // Sistemin anlık işlem kimliğini (Activity ID) veya HTTP isteğinin takip kimliğini yakalıyoruz
            // Bu ID, canlı sunucularda (Production) log kayıtlarından hatayı bulmamızı sağlar.
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}