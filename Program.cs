// Program.cs - Uygulama baslangiç dosyasi
// Online Randevu Sistemi

using AppointmentSystemFinal.Data;
using AppointmentSystemFinal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext ekleniyor (InMemory - hizli calistirma, gecici veritabanı)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AppointmentSystemDb"));

// Identity sistemi (kullanici girisi icin)
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ========== MVC AYARLARI ==========
// Controller ve View desteği ekle
builder.Services.AddControllersWithViews();
// Razor Pages desteği ekle (Identity sayfaları için gerekli)
builder.Services.AddRazorPages();

// Uygulamayı build et (middleware pipeline'ı oluştur)
// Uygulamayı build et (middleware pipeline'ı oluştur)
var app = builder.Build();


// Uygulama basladiginda veritabanini hazirla
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Veritabanini hazirla
    try
    {
        context.Database.EnsureCreated();
        Console.WriteLine("InMemory veritabani hazirlandi.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hata: {ex.Message}");
    }

    // Rolleri olustur (Admin, Staff, Customer)
    try
    {
        string[] roleNames = { "Admin", "Staff", "Customer" };
        
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"'{roleName}' rolu olusturuldu.");
            }
        }

        // Ilk admin kullanicisi
        var adminEmail = "admin@site.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,          // Email onaylanmış olarak işaretle
                FirstName = "Sistem",
                LastName = "Yöneticisi"
            };
            
            // Sifre: Admin123!
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin olusturuldu: {adminEmail}");
            }
            else
            {
                Console.WriteLine("Admin olusturulamadi.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hata: {ex.Message}");
    }
}

// Middleware ayarlari
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS ve HTTPS yönlendirmeyi devre dışı bırakıyoruz (lokalde sadece HTTP ile çalışıyoruz)
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Route ayarlari
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
