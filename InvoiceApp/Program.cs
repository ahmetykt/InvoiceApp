using Microsoft.EntityFrameworkCore;

namespace InvoiceApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1. UYGULAMA OLUŞTURUCU (BUILDER) AŞAMASI
            // Projenin temel yapılandırmasını ve servislerini yönetecek nesneyi başlatıyoruz
            var builder = WebApplication.CreateBuilder(args);

            // Uygulamamızın mimarisi olarak MVC yerine Razor Pages kullanacağını sisteme bildiriyoruz
            builder.Services.AddRazorPages();

            // Dependency Injection (DI) konteynerine veritabanı bağlamımızı (DbContext) ekliyoruz.
            // Bu sayede istediğimiz sayfada 'ApplicationDbContext' nesnesini constructor üzerinden çağırabileceğiz.
            builder.Services.AddDbContext<Services.ApplicationDbContext>(options =>
            {
                // appsettings.json dosyasından "DefaultConnection" adlı SQL bağlantı cümlemizi (Connection String) okuyoruz
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // 2. UYGULAMAYI İNŞA ETME (BUILD) AŞAMASI
            // Servis kayıtları bittikten sonra web uygulamasını ayağa kaldırıyoruz
            var app = builder.Build();

            // 3. MIDDLEWARE (ARA KATMAN) İŞLEMLERİ (HTTP Request Pipeline)

            // Eğer geliştirme (Development) ortamında değilsek (Yani canlı sunucudaysak)
            // Detaylı hata sayfalarını gizle ve kullanıcıyı standart "/Error" sayfasına yönlendir (Güvenlik önlemi)
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            // Gelen isteklerin (URL) hangi sayfaya veya koda gideceğini çözen mekanizma
            app.UseRouting();

            // Kullanıcı yetkilendirme (Authorization) mekanizmasını devreye alıyoruz
            app.UseAuthorization();

            // CSS, JS, Resimler gibi statik dosyaların (wwwroot) modern ve optimize bir şekilde sunulmasını sağlar
            app.MapStaticAssets();

            // Projedeki tüm Razor sayfalarının (Pages klasörü altındaki) rotalarını haritalar
            app.MapRazorPages()
               .WithStaticAssets();

            // Uygulamayı çalıştır ve gelen istekleri dinlemeye başla
            app.Run();
        }
    }
}