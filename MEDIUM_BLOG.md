# ASP.NET Core MVC ile Online Randevu Yönetim Sistemi Geliştirdim — İşte Detayları

> Modern web teknolojileri kullanarak rol tabanlı bir randevu yönetim sistemi nasıl geliştirilir? Bu yazıda, ASP.NET Core 8.0 MVC, Entity Framework Core ve Identity kullanarak geliştirdiğim projeyi detaylı olarak anlatıyorum.

![Cover Image](https://images.unsplash.com/photo-1484480974693-6ca0a78fb36b?w=1200)

## 🎯 Problem: Neden Bu Projeyi Geliştirdim?

Günlük hayatımızda sürekli randevu alıyoruz: doktora, kuaföre, danışmanlara... Peki bu süreç ne kadar verimsiz değil mi? Telefonla aramalar, not defterleri, unutulan randevular, çakışan saatler...

Randevu yönetimi, birçok işletme için kritik bir süreç. Ancak çoğu küçük işletme hala Excel tabloları veya kağıt-kalem ile bu işi yönetmeye çalışıyor. Ben de bu soruna modern bir çözüm geliştirmek istedim.

**Hedef:** Üç farklı kullanıcı tipi (Müşteri, Personel, Yönetici) için özelleştirilmiş, güvenli ve kullanımı kolay bir web uygulaması.

## 🏗️ Teknoloji Seçimleri

Projeyi geliştirirken şu teknolojileri kullandım:

### Backend
- **ASP.NET Core 8.0 MVC**: Microsoft'un modern, cross-platform web framework'ü
- **Entity Framework Core**: Code First yaklaşımı ile veritabanı yönetimi
- **ASP.NET Core Identity**: Kullanıcı kimlik doğrulama ve rol yönetimi
- **SQL Server LocalDB**: Geliştirme ortamı için

### Frontend
- **Razor View Engine**: Server-side rendering
- **Bootstrap 5**: Responsive UI
- **Chart.js**: Raporlama için grafik desteği
 - **Öğrenci Dostu Tema**: Pastel arka plan, yuvarlatılmış kartlar ve pill etiketler ile sade, anlaşılır bir arayüz

### Neden Bu Stack?

1. **ASP.NET Core**: Hızlı, güvenli ve ölçeklenebilir
2. **Entity Framework**: ORM ile veritabanı işlemlerini kolaylaştırır
3. **Identity**: Hazır authentication/authorization sistemi
4. **MVC Pattern**: Kodun düzenli ve bakımının kolay olması

## 🔄 Güncellemeler (v1.1)

- Arayüzü sade ve öğrenci işi bir temaya taşıdım (pastel arka plan, yuvarlatılmış kartlar, pill etiketler, daha yumuşak butonlar).
- Ana sayfaya "3 Adımda Başla" ve "Roller ve Yetkiler" bölümleri ekledim; uygulamanın akışı daha anlaşılır.
- Lokal demo için InMemory veritabanı modunu ekledim (veriler kalıcı değil, sadece hızlı deneme için). Kalıcı SQL Server kullanımı `appsettings.json` ve `UseSqlServer` ile devam ediyor.
- Yerel çalıştırmada HTTPS yönlendirmeyi kapattım; HTTP ile `http://localhost:5012` üzerinden demo kullanılabiliyor.

## 🎭 Rol Tabanlı Yetkilendirme: Sistemin Kalbi

Sistemde üç farklı rol var ve her birinin farklı yetkileri mevcut:

### 1. Customer (Müşteri)
- ✅ Yeni randevu oluşturabilir
- ✅ Kendi randevularını görüntüleyebilir
- ✅ Bekleyen randevularını düzenleyebilir
- ✅ Randevularını iptal edebilir

### 2. Staff (Personel)
- ✅ Kendisine atanan randevuları görüntüler
- ✅ Randevuları onaylar/reddeder
- ✅ Randevulara not ekler
- ❌ Yeni randevu oluşturamaz

### 3. Admin (Yönetici)
- ✅ Tüm randevuları görüntüler ve yönetir
- ✅ Kullanıcıları yönetir
- ✅ Roller atar/kaldırır
- ✅ Sistem raporlarına erişir

## 💾 Veritabanı Tasarımı

Veritabanında iki ana entity kullandım:

### ApplicationUser (Kullanıcı)
```csharp
public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    
    // Navigation Properties
    public virtual ICollection<Appointment> CustomerAppointments { get; set; }
    public virtual ICollection<Appointment> StaffAppointments { get; set; }
}
```

Identity'nin `IdentityUser` sınıfını extend ederek, kullanıcıya **Ad** ve **Soyad** alanları ekledim.

### Appointment (Randevu)
```csharp
public class Appointment
{
    public int Id { get; set; }
    
    [Required]
    public string CustomerId { get; set; }  // Müşteri
    public virtual ApplicationUser Customer { get; set; }
    
    public string? StaffId { get; set; }  // Personel (nullable)
    public virtual ApplicationUser? Staff { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public TimeSpan StartTime { get; set; }
    
    [Required]
    public TimeSpan EndTime { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; }
    
    public string? Description { get; set; }
    
    [Required]
    public string Status { get; set; } = "Pending";
    
    public string? StaffNote { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

**İlişkiler:**
- Bir müşterinin birden fazla randevusu olabilir (1:N)
- Bir personele birden fazla randevu atanabilir (1:N)

## 🔐 Güvenlik ve Yetkilendirme

### Rol Bazlı Controller Koruması

```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    // Sadece Admin erişebilir
}
```

### Metod Seviyesinde Yetkilendirme

```csharp
[Authorize(Roles = "Customer")]
public IActionResult Create()
{
    // Sadece müşteriler randevu oluşturabilir
    return View();
}
```

### Manuel Yetki Kontrolü

```csharp
public async Task<IActionResult> Edit(int? id)
{
    var appointment = await _context.Appointments.FindAsync(id);
    var currentUser = await _userManager.GetUserAsync(User);
    
    // Müşteri sadece kendi randevusunu düzenleyebilir
    if (!User.IsInRole("Admin") && 
        appointment.CustomerId != currentUser.Id)
    {
        return Forbid();
    }
    
    return View(appointment);
}
```

## 🎨 Randevu Yönetimi: İş Akışı

### 1. Müşteri Randevu Oluşturur

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Customer")]
public async Task<IActionResult> Create(Appointment appointment)
{
    if (ModelState.IsValid)
    {
        // Otomatik alanları doldur
        var currentUser = await _userManager.GetUserAsync(User);
        appointment.CustomerId = currentUser.Id;
        appointment.Status = "Pending";
        appointment.CreatedAt = DateTime.Now;
        
        // Validasyonlar
        if (appointment.Date < DateTime.Today)
        {
            ModelState.AddModelError("Date", "Geçmiş tarih seçilemez");
            return View(appointment);
        }
        
        if (appointment.EndTime <= appointment.StartTime)
        {
            ModelState.AddModelError("EndTime", 
                "Bitiş saati başlangıç saatinden büyük olmalı");
            return View(appointment);
        }
        
        _context.Add(appointment);
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Randevu başarıyla oluşturuldu!";
        return RedirectToAction(nameof(Index));
    }
    
    return View(appointment);
}
```

### 2. Personel Onaylar/Reddeder

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Staff,Admin")]
public async Task<IActionResult> ChangeStatus(int id, 
    string status, string? staffNote)
{
    var appointment = await _context.Appointments.FindAsync(id);
    
    if (appointment == null)
        return NotFound();
    
    appointment.Status = status;
    appointment.StaffNote = staffNote;
    appointment.UpdatedAt = DateTime.Now;
    
    await _context.SaveChangesAsync();
    
    TempData["Success"] = "Randevu durumu güncellendi!";
    return RedirectToAction(nameof(Index));
}
```

### 3. Rol Bazlı Listeleme

```csharp
public async Task<IActionResult> Index()
{
    var currentUser = await _userManager.GetUserAsync(User);
    
    IQueryable<Appointment> query = _context.Appointments
        .Include(a => a.Customer)
        .Include(a => a.Staff);
    
    if (User.IsInRole("Admin"))
    {
        // Admin tüm randevuları görür
    }
    else if (User.IsInRole("Staff"))
    {
        // Personel sadece kendine atananları görür
        query = query.Where(a => a.StaffId == currentUser.Id);
    }
    else
    {
        // Müşteri sadece kendininkileri görür
        query = query.Where(a => a.CustomerId == currentUser.Id);
    }
    
    var appointments = await query
        .OrderByDescending(a => a.Date)
        .ToListAsync();
    
    return View(appointments);
}
```

## 📊 Raporlama Sistemi

Admin için grafik destekli raporlama ekledim:

```csharp
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Report()
{
    var totalAppointments = await _context.Appointments.CountAsync();
    
    var statusCounts = await _context.Appointments
        .GroupBy(a => a.Status)
        .Select(g => new { Status = g.Key, Count = g.Count() })
        .ToListAsync();
    
    // Son 7 gün için grafik verisi
    var last7Days = await _context.Appointments
        .Where(a => a.CreatedAt >= DateTime.Today.AddDays(-7))
        .GroupBy(a => a.CreatedAt.Date)
        .Select(g => new { Date = g.Key, Count = g.Count() })
        .OrderBy(x => x.Date)
        .ToListAsync();
    
    ViewBag.ChartLabels = last7Days.Select(x => x.Date.ToString("dd MMM"));
    ViewBag.ChartData = last7Days.Select(x => x.Count);
    
    return View();
}
```

View'da Chart.js ile görselleştirme:

```javascript
<script>
var ctx = document.getElementById('appointmentChart').getContext('2d');
var chart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: @Html.Raw(Json.Serialize(ViewBag.ChartLabels)),
        datasets: [{
            label: 'Randevu Sayısı',
            data: @Html.Raw(Json.Serialize(ViewBag.ChartData)),
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
        }]
    }
});
</script>
```

## 🚀 Seed Data: İlk Çalıştırma

Uygulama ilk çalıştığında otomatik olarak roller ve admin kullanıcısı oluşturuluyor:

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    
    // Rolleri oluştur
    string[] roleNames = { "Admin", "Staff", "Customer" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    
    // Admin kullanıcısı oluştur
    var adminEmail = "admin@site.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Sistem",
            LastName = "Yöneticisi"
        };
        
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
```

## 🎨 Kullanıcı Arayüzü

Bootstrap 5 ile responsive bir arayüz tasarladım. Önemli noktalar:

### Durum Badge'leri

```csharp
// Appointment.cs - Computed Property
public string StatusColor
{
    get
    {
        return Status switch
        {
            "Pending" => "warning",
            "Approved" => "success",
            "Rejected" => "danger",
            "Cancelled" => "secondary",
            _ => "info"
        };
    }
}
```

View'da kullanımı:

```html
<span class="badge bg-@Model.StatusColor">
    @Model.StatusDisplay
</span>
```

### Rol Bazlı Menü

Ana sayfada öğrenci dostu bir açılış bölümü, hızlı aksiyon butonları ve rol/akış özetleri bulunuyor. Arayüz Bootstrap 5 üzerine kurulu ve sade.

## ▶️ Hızlı Çalıştırma (Demo)

Yerel demo için InMemory modunu açtım:

```bash
cd AppointmentSystemFinal
dotnet run --urls=http://localhost:5012
```

Sonra tarayıcıdan `http://localhost:5012` ile erişiyorum. Admin hesabı otomatik geliyor: `admin@site.com / Admin123!`.

Kalıcı veri için SQL Server bağlantısını `appsettings.json` ile ayarlayıp `UseSqlServer` yapılandırmasını kullanıyorum.

```html
@if (User.IsInRole("Admin"))
{
    <li class="nav-item dropdown">
        <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
            Admin
        </a>
        <ul class="dropdown-menu">
            <li><a class="dropdown-item" asp-controller="Admin" asp-action="Users">Kullanıcılar</a></li>
            <li><a class="dropdown-item" asp-controller="Admin" asp-action="Report">Raporlar</a></li>
        </ul>
    </li>
}
```

## 📝 Validation: Veri Doğrulama

### Model Seviyesi

```csharp
[Required(ErrorMessage = "Randevu tarihi zorunludur")]
[DataType(DataType.Date)]
[Display(Name = "Randevu Tarihi")]
public DateTime Date { get; set; }

[Required(ErrorMessage = "Başlık zorunludur")]
[StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
public string Title { get; set; }
```

### Controller Seviyesi

```csharp
if (appointment.Date < DateTime.Today)
{
    ModelState.AddModelError("Date", "Geçmiş tarih seçilemez");
}

if (appointment.EndTime <= appointment.StartTime)
{
    ModelState.AddModelError("EndTime", 
        "Bitiş saati başlangıç saatinden büyük olmalı");
}
```

### Client-Side Validation

```html
<form asp-action="Create" method="post">
    <div asp-validation-summary="ModelOnly" class="text-danger"></div>
    
    <div class="form-group">
        <label asp-for="Date"></label>
        <input asp-for="Date" class="form-control" />
        <span asp-validation-for="Date" class="text-danger"></span>
    </div>
    
    <!-- ... -->
    
    <button type="submit" class="btn btn-primary">Kaydet</button>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

## 🔄 Entity Framework: Migration İşlemleri

Veritabanını Code First yaklaşımıyla oluşturdum:

```bash
# Migration oluştur
dotnet ef migrations add InitialCreate

# Veritabanını güncelle
dotnet ef database update
```

**DbContext Konfigürasyonu:**

```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Appointment> Appointments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Customer -> Appointments ilişkisi
        builder.Entity<Appointment>()
            .HasOne(a => a.Customer)
            .WithMany(u => u.CustomerAppointments)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Staff -> Appointments ilişkisi
        builder.Entity<Appointment>()
            .HasOne(a => a.Staff)
            .WithMany(u => u.StaffAppointments)
            .HasForeignKey(a => a.StaffId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // İndeksler
        builder.Entity<Appointment>()
            .HasIndex(a => a.CustomerId);
        
        builder.Entity<Appointment>()
            .HasIndex(a => a.StaffId);
        
        builder.Entity<Appointment>()
            .HasIndex(a => a.Date);
    }
}
```

## 🎯 Karşılaştığım Zorluklar ve Çözümler

### 1. Rol Bazlı Filtreleme
**Problem:** Her rol için farklı veri setlerini göstermek.  
**Çözüm:** LINQ'da `IQueryable` kullanarak sorguyu dinamik olarak şekillendirdim.

### 2. Navigation Properties
**Problem:** Bir kullanıcının hem Customer hem Staff olarak randevuları.  
**Çözüm:** İki ayrı navigation property (`CustomerAppointments`, `StaffAppointments`) tanımladım.

### 3. Identity Customization
**Problem:** IdentityUser'a ek alanlar eklemek.  
**Çözüm:** `ApplicationUser` sınıfı oluşturup `IdentityUser`'dan türettim.

## 📈 Performans İyileştirmeleri

### Eager Loading
```csharp
var appointments = await _context.Appointments
    .Include(a => a.Customer)
    .Include(a => a.Staff)
    .ToListAsync();
```

### Asenkron İşlemler
```csharp
public async Task<IActionResult> Index()
{
    var data = await _context.Appointments.ToListAsync();
    return View(data);
}
```

### İndeksleme
```csharp
builder.Entity<Appointment>()
    .HasIndex(a => a.CustomerId);
```

## 🚀 Gelecek Planları

Projeyi daha da geliştirmek için planlarım:

### 1. Bildirim Sistemi
- ✉️ E-posta bildirimleri
- 📱 SMS entegrasyonu
- 🔔 Push notifications

### 2. Takvim Görünümü
- 📅 FullCalendar.js entegrasyonu
- 🔄 Randevu çakışma kontrolü
- 📆 Google Calendar senkronizasyonu

### 3. Ödeme Sistemi
- 💳 Online ödeme entegrasyonu
- 🧾 Fatura oluşturma
- 💰 Randevu ücreti yönetimi

### 4. API Geliştirme
- 🔌 RESTful API
- 📱 Mobil uygulama desteği
- 🔐 JWT authentication

### 5. Gelişmiş Özellikler
- 🌍 Multi-language desteği
- 📊 Daha detaylı raporlama
- 🔒 Two-Factor Authentication
- 📤 PDF/Excel export

## 💡 Öğrendiklerim

Bu proje boyunca:

1. **ASP.NET Core MVC** mimarisini derinlemesine öğrendim
2. **Entity Framework Core** ile ilişkisel veritabanı yönetimini kavradım
3. **ASP.NET Core Identity** ile güvenli authentication/authorization implementasyonu yaptım
4. **Dependency Injection** ve servis yaşam döngülerini anladım
5. **LINQ** sorguları ile veri manipülasyonunda ustalaştım
6. **Razor View Engine** ile dynamic HTML üretimini öğrendim
7. **Bootstrap 5** ile responsive tasarım becerilerimi geliştirdim

## 📦 Proje İstatistikleri

- **Toplam Kod Satırı:** ~2000+
- **Controller:** 3 adet
- **Model:** 2 adet (+ Identity models)
- **View:** 20+ sayfa
- **Veritabanı Tablosu:** 7 (Identity + Appointments)
- **Geliştirme Süresi:** 2 hafta

## 🎓 Sonuç

Bu proje, modern web geliştirme teknolojilerini kullanarak gerçek dünya problemlerine çözüm üretmenin harika bir örneği oldu. ASP.NET Core'un gücü, Entity Framework'ün kolaylığı ve Identity'nin güvenliği bir araya gelince ortaya sağlam bir uygulama çıktı.

En önemlisi, **rol tabanlı yetkilendirme** ve **CRUD işlemlerinin** temiz bir şekilde nasıl implement edileceğini öğrendim. Bu deneyim, gelecek projelerimde bana büyük avantaj sağlayacak.

Eğer siz de benzer bir proje geliştirmeyi düşünüyorsanız, ASP.NET Core'u şiddetle tavsiye ederim. Dokümantasyonu mükemmel, community desteği güçlü ve Microsoft'un arkasında olması büyük bir güven veriyor.

---

## 🔗 Kaynaklar ve Linkler

- **Microsoft Docs:** https://docs.microsoft.com/aspnet/core
- **Entity Framework Core:** https://docs.microsoft.com/ef/core
- **ASP.NET Core Identity:** https://docs.microsoft.com/aspnet/core/security/authentication/identity
- **Bootstrap 5:** https://getbootstrap.com
- **Chart.js:** https://www.chartjs.org

---

## 💬 Sorularınız mı var?

Proje hakkında sorularınız veya önerileriniz varsa yorumlarda paylaşabilirsiniz. Kod örneklerini genişletmemi veya belirli bir konuyu daha detaylı anlatmamı isterseniz memnuniyetle yazarım!

**Mutlu kodlamalar! 🚀**

---

### Etiketler
`#aspnetcore` `#csharp` `#webdevelopment` `#entityframework` `#programming` `#dotnet` `#mvc` `#coding` `#softwaredevelopment` `#backend`

---

**Yazar:** Ferhat Bayutmus  
**Tarih:** 17 Kasım 2025  
**Proje:** Online Randevu Yönetim Sistemi
