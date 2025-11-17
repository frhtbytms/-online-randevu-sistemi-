# ONLİNE RANDEVU YÖNETİM SİSTEMİ

**Final Projesi Raporu**

---

**Öğrenci Adı Soyadı:** Ferhat  Bayutmus
**Ders Adı:** İnternet Programcılığı  
**Tarih:** 17 Kasım 2025  


---

## ÖZET

Bu çalışmada, ASP.NET Core 8.0 MVC framework'ü kullanılarak geliştirilmiş web tabanlı bir randevu yönetim sistemi sunulmaktadır. Sistem, üç farklı kullanıcı rolü (Admin, Staff, Customer) ile rol tabanlı yetkilendirme mekanizması üzerine inşa edilmiştir. Entity Framework Core Code First yaklaşımı ile veritabanı tasarımı gerçekleştirilmiş, ASP.NET Core Identity ile güvenli kimlik doğrulama ve yetkilendirme sağlanmıştır.

Proje kapsamında kullanıcılar, sistemde randevu oluşturabilir, görüntüleyebilir, düzenleyebilir ve silebilir. Personel kullanıcıları kendilerine atanan randevuları onaylayabilir veya reddedebilir. Yönetici kullanıcılar ise tüm sistemi yönetebilir, kullanıcı rollerini düzenleyebilir ve detaylı raporlara erişebilir. Sistemin geliştirilmesinde modern web teknolojileri ve yazılım mühendisliği prensipleri uygulanmış, kullanıcı dostu bir arayüz tasarlanmıştır.

**Anahtar Kelimeler:** ASP.NET Core MVC, Entity Framework Core, Rol Tabanlı Yetkilendirme, Randevu Yönetimi, Web Uygulaması

---

## 1. GİRİŞ

### 1.1. Projenin Amacı

Günümüzde dijitalleşen iş süreçleri ile birlikte randevu yönetim sistemlerine olan ihtiyaç giderek artmaktadır. Hastaneler, klinikler, güzellik salonları, danışmanlık firmaları gibi birçok sektörde randevu sistemleri kritik öneme sahiptir. Bu proje, modern web teknolojileri kullanılarak geliştirilen, güvenli ve kullanıcı dostu bir randevu yönetim sisteminin tasarım ve implementasyonunu kapsamaktadır.

### 1.2. Problem Tanımı

Geleneksel randevu yönetim süreçlerinde yaşanan sorunlar şunlardır:
- Manuel kayıt tutmanın zaman alıcı ve hata yapma olasılığının yüksek olması
- Randevu çakışmalarının önlenememesi
- Müşteri ve personel arasında etkili iletişim eksikliği
- Raporlama ve analiz imkanlarının sınırlı olması
- Farklı kullanıcı seviyelerine göre erişim kontrolünün yetersizliği

### 1.3. Projenin Kapsamı

Bu proje, yukarıda belirtilen sorunlara çözüm getirmek amacıyla geliştirilmiş web tabanlı bir randevu yönetim sistemidir. Sistem, üç farklı kullanıcı rolü için özel olarak tasarlanmış özelliklere sahiptir:

- **Müşteri (Customer):** Randevu oluşturma, görüntüleme ve yönetme
- **Personel (Staff):** Atanan randevuları onaylama/reddetme ve not ekleme
- **Yönetici (Admin):** Sistem genelinde tam yetki, kullanıcı ve rol yönetimi, raporlama

### 1.4. Projenin Önemi

Bu proje, aşağıdaki açılardan önem taşımaktadır:
1. Modern web geliştirme teknolojilerinin uygulamalı olarak öğrenilmesi
2. Rol tabanlı yetkilendirme sisteminin implementasyonu
3. Veritabanı tasarımı ve ORM (Object-Relational Mapping) kullanımı
4. Güvenli kimlik doğrulama ve yetkilendirme mekanizmalarının uygulanması
5. Gerçek dünya iş problemlerine yazılım çözümleri geliştirilmesi

---

## 2. MATERYAL VE METOT

### 2.1. Kullanılan Teknolojiler

#### 2.1.1. Backend Teknolojileri

**ASP.NET Core 8.0 MVC**
- Microsoft tarafından geliştirilen, açık kaynak kodlu, platform bağımsız web framework'ü
- Model-View-Controller (MVC) tasarım desenini uygular
- Yüksek performans ve ölçeklenebilirlik sağlar
- Dependency Injection (Bağımlılık Enjeksiyonu) desteği sunar

**Entity Framework Core 8.0**
- Object-Relational Mapping (ORM) framework'ü
- Code First yaklaşımı ile veritabanı tasarımı
- LINQ (Language Integrated Query) desteği
- Migration (göç) sistemi ile veritabanı versiyonlama

**ASP.NET Core Identity**
- Kullanıcı kimlik doğrulama ve yetkilendirme sistemi
- Şifre hashleme ve güvenlik önlemleri
- Rol tabanlı yetkilendirme (Role-Based Authorization)
- Cookie tabanlı oturum yönetimi

#### 2.1.2. Frontend Teknolojileri

**Razor View Engine**
- Sunucu taraflı HTML üretimi
- C# kodunun HTML içinde kullanılması
- Strongly-typed view desteği

**Bootstrap 5**
- Responsive (duyarlı) tasarım framework'ü
- Hazır UI bileşenleri
- Grid sistem ile esnek sayfa düzeni

**Chart.js**
- JavaScript tabanlı grafik kütüphanesi
- İnteraktif ve görsel raporlar için kullanılmıştır

#### 2.1.3. Veritabanı

**Microsoft SQL Server LocalDB**
- Geliştirme ortamı için hafif SQL Server sürümü
- Tam SQL Server özelliklerini destekler
- Kolay kurulum ve yapılandırma

#### 2.1.4. Geliştirme Ortamı

- **IDE:** Visual Studio 2022 / Visual Studio Code
- **Framework:** .NET 8.0 SDK
- **Versiyon Kontrol:** Git
- **Programlama Dili:** C# 12

### 2.2. Sistem Mimarisi

#### 2.2.1. MVC (Model-View-Controller) Tasarım Deseni

Proje, MVC tasarım desenini takip etmektedir:

**Model (Model)**
- Uygulamanın veri yapısını temsil eder
- İş mantığını içerir
- Veritabanı ile etkileşimi sağlar
- Örnekler: `ApplicationUser.cs`, `Appointment.cs`

**View (Görünüm)**
- Kullanıcı arayüzünü oluşturur
- Razor syntax ile dinamik HTML üretimi
- Model verilerini görsel olarak sunar
- Örnekler: `Index.cshtml`, `Create.cshtml`

**Controller (Kontrolcü)**
- Kullanıcı isteklerini işler
- Model ve View arasında köprü görevi görür
- İş akışını yönetir
- Örnekler: `AppointmentsController.cs`, `AdminController.cs`

#### 2.2.2. Katmanlı Mimari

```
┌─────────────────────────────────┐
│   Presentation Layer (Views)    │  <- Kullanıcı Arayüzü
├─────────────────────────────────┤
│   Application Layer              │  <- Controllers
│   (Business Logic)               │
├─────────────────────────────────┤
│   Data Access Layer              │  <- DbContext, Repositories
│   (Entity Framework Core)        │
├─────────────────────────────────┤
│   Database Layer                 │  <- SQL Server LocalDB
│   (SQL Server)                   │
└─────────────────────────────────┘
```

### 2.3. Veritabanı Tasarımı

#### 2.3.1. Varlık İlişki (Entity-Relationship) Modeli

Sistemde iki ana varlık bulunmaktadır:

**1. ApplicationUser (Kullanıcı)**
- ASP.NET Core Identity'nin `IdentityUser` sınıfından türetilmiştir
- Ek özellikler: FirstName (Ad), LastName (Soyad)
- Hesaplanan özellik: FullName (Ad Soyad birleşimi)

**2. Appointment (Randevu)**
- Randevu bilgilerini tutar
- İki adet Foreign Key içerir: CustomerId ve StaffId

#### 2.3.2. Tablo Yapıları

**AspNetUsers Tablosu (Kullanıcılar)**
```
┌──────────────────┬──────────────┬────────────────┐
│ Sütun Adı        │ Veri Tipi    │ Açıklama       │
├──────────────────┼──────────────┼────────────────┤
│ Id               │ nvarchar(450)│ Primary Key    │
│ FirstName        │ nvarchar(50) │ Ad             │
│ LastName         │ nvarchar(50) │ Soyad          │
│ UserName         │ nvarchar(256)│ Kullanıcı adı  │
│ Email            │ nvarchar(256)│ E-posta        │
│ PasswordHash     │ nvarchar(MAX)│ Şifre (hash)   │
│ ...              │ ...          │ (Identity)     │
└──────────────────┴──────────────┴────────────────┘
```

**Appointments Tablosu (Randevular)**
```
┌──────────────────┬──────────────┬────────────────────────┐
│ Sütun Adı        │ Veri Tipi    │ Açıklama               │
├──────────────────┼──────────────┼────────────────────────┤
│ Id               │ int          │ Primary Key (Identity) │
│ CustomerId       │ nvarchar(450)│ FK -> AspNetUsers      │
│ StaffId          │ nvarchar(450)│ FK -> AspNetUsers      │
│ Date             │ datetime2    │ Randevu tarihi         │
│ StartTime        │ time         │ Başlangıç saati        │
│ EndTime          │ time         │ Bitiş saati            │
│ Title            │ nvarchar(200)│ Randevu başlığı        │
│ Description      │ nvarchar(1000)│ Açıklama              │
│ Status           │ nvarchar(50) │ Durum                  │
│ StaffNote        │ nvarchar(500)│ Personel notu          │
│ CreatedAt        │ datetime2    │ Oluşturulma tarihi     │
│ UpdatedAt        │ datetime2    │ Güncellenme tarihi     │
└──────────────────┴──────────────┴────────────────────────┘
```

#### 2.3.3. İlişkiler (Relationships)

**1. Customer -> Appointments (1:N)**
- Bir müşteri birden fazla randevu oluşturabilir
- Silme kuralı: Restrict (Müşteri silinirse randevular silinmez, hata verir)

**2. Staff -> Appointments (1:N)**
- Bir personele birden fazla randevu atanabilir
- Silme kuralı: SetNull (Personel silinirse randevulardaki StaffId null olur)

**ER Diyagramı:**
```
┌─────────────────┐            ┌─────────────────┐
│  AspNetUsers    │            │  Appointments   │
│  (Customer)     │            │                 │
├─────────────────┤            ├─────────────────┤
│ Id (PK)         │───────────>│ CustomerId (FK) │
│ FirstName       │     1:N    │ StaffId (FK)    │
│ LastName        │            │ Date            │
│ Email           │<───────────│ StartTime       │
│ ...             │            │ EndTime         │
└─────────────────┘     1:N    │ Title           │
 (Staff)                       │ Status          │
                               │ ...             │
                               └─────────────────┘
```

### 2.4. Rol Tabanlı Yetkilendirme

Sistemde üç rol tanımlanmıştır:

#### 2.4.1. Admin (Yönetici)
**Yetkiler:**
- Tüm kullanıcıları görüntüleme
- Kullanıcılara rol atama/kaldırma
- Kullanıcı silme (kendi hesabı hariç)
- Tüm randevuları görüntüleme ve yönetme
- Sistem raporlarına erişim
- Personel performans raporlarını görüntüleme

**Controller Yetkilendirmesi:**
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller { ... }
```

#### 2.4.2. Staff (Personel)
**Yetkiler:**
- Kendisine atanan randevuları görüntüleme
- Randevu durumunu değiştirme (Onaylama/Reddetme)
- Randevulara not ekleme
- Kendi profilini düzenleme

**Metod Yetkilendirmesi:**
```csharp
[Authorize(Roles = "Staff,Admin")]
public async Task<IActionResult> ChangeStatus(int? id) { ... }
```

#### 2.4.3. Customer (Müşteri)
**Yetkiler:**
- Yeni randevu oluşturma
- Kendi randevularını görüntüleme
- Beklemede olan randevularını düzenleme
- Randevularını silme/iptal etme
- Kendi profilini düzenleme

**Metod Yetkilendirmesi:**
```csharp
[Authorize(Roles = "Customer")]
public IActionResult Create() { ... }
```

### 2.5. Güvenlik Önlemleri

#### 2.5.1. Kimlik Doğrulama (Authentication)
- Cookie tabanlı oturum yönetimi
- Şifre hashleme (PBKDF2 algoritması)
- Minimum şifre gereksinimleri

#### 2.5.2. Yetkilendirme (Authorization)
- Rol tabanlı erişim kontrolü
- Attribute bazlı yetkilendirme
- Manuel yetki kontrolleri (Örn: Kullanıcı sadece kendi randevusunu görebilir)

#### 2.5.3. CSRF Koruması
- Anti-Forgery Token kullanımı
- Form gönderimlerinde token validasyonu

```csharp
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Appointment appointment) { ... }
```

---

## 3. UYGULAMA

### 3.1. Uygulama Geliştirme Aşamaları

#### 3.1.1. Proje Oluşturma ve Yapılandırma

**Adım 1: ASP.NET Core MVC Projesi Oluşturma**
```bash
dotnet new mvc -n AppointmentSystemFinal --auth Individual
```

**Adım 2: NuGet Paketlerinin Yüklenmesi**
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Identity.EntityFrameworkCore

**Adım 3: Connection String Yapılandırması**
`appsettings.json` dosyasında veritabanı bağlantı dizesi tanımlanmıştır:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AppointmentSystemDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### 3.1.2. Model Sınıflarının Oluşturulması

**ApplicationUser.cs**
- IdentityUser sınıfından türetilmiştir
- FirstName ve LastName özellikleri eklenmiştir
- Navigation properties tanımlanmıştır

**Appointment.cs**
- Randevu verilerini tutar
- Data Annotations ile validasyon kuralları tanımlanmıştır
- Computed properties (StatusDisplay, StatusColor) eklenmiştir

#### 3.1.3. DbContext Yapılandırması

`ApplicationDbContext.cs` dosyasında:
- IdentityDbContext<ApplicationUser> sınıfından türetilmiştir
- DbSet<Appointment> tanımlanmıştır
- OnModelCreating metodunda ilişkiler ve kısıtlamalar yapılandırılmıştır

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
    }
}
```

#### 3.1.4. Migration ve Veritabanı Oluşturma

**Migration Oluşturma:**
```bash
dotnet ef migrations add InitialCreate
```

**Veritabanını Güncelleme:**
```bash
dotnet ef database update
```

#### 3.1.5. Seed Verisi Ekleme

`Program.cs` dosyasında uygulama başlatıldığında:
- Admin, Staff, Customer rolleri oluşturulur
- Varsayılan admin kullanıcısı oluşturulur (admin@site.com / Admin123!)

### 3.2. Controller Implementasyonları

#### 3.2.1. AppointmentsController

**Index (Listeleme) Metodu**
```csharp
public async Task<IActionResult> Index()
{
    var currentUser = await _userManager.GetUserAsync(User);
    
    IQueryable<Appointment> query = _context.Appointments
        .Include(a => a.Customer)
        .Include(a => a.Staff);
    
    if (User.IsInRole("Admin"))
    {
        // Tüm randevular
    }
    else if (User.IsInRole("Staff"))
    {
        query = query.Where(a => a.StaffId == currentUser.Id);
    }
    else
    {
        query = query.Where(a => a.CustomerId == currentUser.Id);
    }
    
    return View(await query.ToListAsync());
}
```

**Create (Oluşturma) Metodu**
- Sadece Customer rolü erişebilir
- Tarih validasyonu (geçmiş tarih kontrolü)
- Saat validasyonu (EndTime > StartTime)
- Otomatik alan doldurmalar (CustomerId, Status, CreatedAt)

**Edit (Düzenleme) Metodu**
- Sahiplik kontrolü (Kullanıcı sadece kendi randevusunu düzenleyebilir)
- Admin tüm randevuları düzenleyebilir

**Delete (Silme) Metodu**
- Yetkilendirme kontrolü
- Cascade delete işlemleri

**ChangeStatus (Durum Değiştirme) Metodu**
- Staff ve Admin erişebilir
- Durum güncelleme (Pending -> Approved/Rejected)
- Personel notu ekleme

#### 3.2.2. AdminController

**Users (Kullanıcı Listesi) Metodu**
```csharp
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Users()
{
    var users = await _userManager.Users.ToListAsync();
    
    var userViewModels = new List<UserViewModel>();
    foreach (var user in users)
    {
        var roles = await _userManager.GetRolesAsync(user);
        userViewModels.Add(new UserViewModel
        {
            User = user,
            Roles = roles
        });
    }
    
    return View(userViewModels);
}
```

**ManageRoles (Rol Yönetimi) Metodu**
- Kullanıcıya rol atama
- Kullanıcıdan rol kaldırma
- Checkbox bazlı rol seçimi

**Report (Raporlama) Metodu**
- Toplam istatistikler
- Durumlara göre sayılar
- Son 7 gün grafik verisi
- Chart.js ile görselleştirme

**StaffReport (Personel Raporu) Metodu**
- Her personelin randevu sayıları
- Onaylanan/reddedilen randevu oranları
- Performans metrikleri

### 3.3. View (Görünüm) Tasarımı

#### 3.3.1. Layout (_Layout.cshtml)

Ana şablon dosyası:
- Navigation bar (Menü)
- Rol bazlı menü öğeleri
- Login/Logout bağlantıları
- Footer

```html
@if (User.IsInRole("Admin"))
{
    <li class="nav-item">
        <a class="nav-link" asp-controller="Admin" asp-action="Users">Admin Panel</a>
    </li>
}
```

#### 3.3.2. Appointments/Index.cshtml

Randevu listesi sayfası:
- Rol bazlı başlık
- Tablo ile randevu listesi
- Durum badge'leri (Bootstrap)
- Aksiyon butonları (Details, Edit, Delete, ChangeStatus)

#### 3.3.3. Appointments/Create.cshtml

Randevu oluşturma formu:
- Tarih seçici (Date picker)
- Saat seçiciler (Time picker)
- Personel dropdown (Staff seçimi)
- Client-side validasyon
- Server-side validasyon

#### 3.3.4. Admin/Report.cshtml

Raporlama sayfası:
- İstatistik kartları (Bootstrap cards)
- Chart.js grafikler
- Son 7 gün randevu grafiği
- Responsive tasarım

### 3.4. Validasyon ve Hata Yönetimi

#### 3.4.1. Model Validasyonu

Data Annotations kullanımı:
```csharp
[Required(ErrorMessage = "Randevu tarihi zorunludur")]
[DataType(DataType.Date)]
[Display(Name = "Randevu Tarihi")]
public DateTime Date { get; set; }

[StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
public string Title { get; set; }
```

#### 3.4.2. Custom Validasyon

Controller'da özel validasyon:
```csharp
if (appointment.Date < DateTime.Today)
{
    ModelState.AddModelError("Date", "Geçmiş tarih seçilemez");
}

if (appointment.EndTime <= appointment.StartTime)
{
    ModelState.AddModelError("EndTime", "Bitiş saati başlangıç saatinden büyük olmalı");
}
```

#### 3.4.3. Hata Mesajları

TempData kullanımı:
```csharp
TempData["Success"] = "Randevu başarıyla oluşturuldu!";
TempData["Error"] = "Bir hata oluştu!";
```

### 3.5. Uygulama Ekran Görüntüleri

#### 3.5.1. Ana Sayfa (Home/Index)
- Hoş geldiniz mesajı
- Sistemin tanıtımı
- Kullanıcı rolüne göre yönlendirme butonları

#### 3.5.2. Login/Register Sayfaları
- ASP.NET Core Identity scaffold sayfaları
- FirstName ve LastName alanları eklenmiş
- Responsive tasarım

#### 3.5.3. Randevu Listesi
**Customer Görünümü:**
- Sadece kendi randevuları
- "Randevularım" başlığı
- Create New butonu

**Staff Görünümü:**
- Atanan randevular
- "Atandığım Randevular" başlığı
- Change Status butonu

**Admin Görünümü:**
- Tüm randevular
- "Tüm Randevular" başlığı
- Tam yönetim yetkileri

#### 3.5.4. Randevu Oluşturma Formu
- Tarih seçici (HTML5 date input)
- Başlangıç/bitiş saat seçiciler
- Randevu başlığı ve açıklama alanları
- Personel seçimi (dropdown)
- Kaydet ve iptal butonları

#### 3.5.5. Admin Paneli
**Kullanıcı Yönetimi:**
- Kullanıcı listesi tablosu
- Rol bilgileri
- Aksiyon butonları (Manage Roles, Delete)

**Raporlar:**
- İstatistik kartları (Toplam randevu, onaylanan, reddedilen vb.)
- Grafik görselleri (Chart.js)
- Tarih filtreleme

**Personel Raporu:**
- Personel bazlı performans metrikleri
- Tablo formatında gösterim

---

## 4. SONUÇ

### 4.1. Projenin Başarıları

Bu proje kapsamında modern web teknolojileri kullanılarak güvenli, ölçeklenebilir ve kullanıcı dostu bir randevu yönetim sistemi başarıyla geliştirilmiştir. Projenin başlıca başarıları şunlardır:

1. **Rol Tabanlı Yetkilendirme:** Üç farklı kullanıcı rolü için özelleştirilmiş yetkilendirme sistemi başarıyla implemente edilmiştir.

2. **Güvenlik:** ASP.NET Core Identity kullanılarak güvenli kimlik doğrulama, şifre hashleme ve CSRF koruması sağlanmıştır.

3. **Veritabanı Tasarımı:** Entity Framework Core Code First yaklaşımı ile normalize edilmiş, ilişkisel bir veritabanı tasarımı gerçekleştirilmiştir.

4. **Kullanıcı Deneyimi:** Bootstrap 5 ile responsive ve kullanıcı dostu bir arayüz tasarlanmıştır.

5. **Raporlama:** Chart.js kullanılarak görsel ve anlaşılır raporlama sistemi oluşturulmuştur.

6. **CRUD İşlemleri:** Randevular için tam CRUD (Create, Read, Update, Delete) işlemleri başarıyla implemente edilmiştir.

7. **Validasyon:** Hem client-side hem de server-side validasyon ile veri bütünlüğü sağlanmıştır.

### 4.2. Karşılaşılan Zorluklar ve Çözümler

**Zorluk 1: Rol Bazlı Filtreleme**
- **Problem:** Farklı roller için farklı veri setlerinin gösterilmesi
- **Çözüm:** LINQ sorguları ve User.IsInRole() kontrolü ile çözüldü

**Zorluk 2: İlişkisel Veri Yönetimi**
- **Problem:** Customer ve Staff için ayrı navigation properties
- **Çözüm:** Entity Framework'te explicit relationship configuration kullanıldı

**Zorluk 3: Tarih ve Saat Validasyonu**
- **Problem:** Geçmiş tarih ve mantıksız saat aralığı kontrolü
- **Çözüm:** Custom validasyon metodları ile çözüldü

**Zorluk 4: Identity Customization**
- **Problem:** IdentityUser'a ek alanlar eklenmesi
- **Çözüm:** ApplicationUser sınıfı oluşturuldu ve migration'larla güncellendi

### 4.3. Sistemin Özellikleri

**Güçlü Yönler:**
- Modern teknoloji stack'i
- Güvenli kimlik doğrulama ve yetkilendirme
- Responsive tasarım (mobil uyumlu)
- Detaylı raporlama
- Kolay bakım ve geliştirilebilirlik
- Code First yaklaşımı ile esnek veritabanı yönetimi

**Geliştirilebilir Yönler:**
- E-posta bildirimleri eklenebilir
- SMS onay sistemi entegre edilebilir
- Randevu çakışma kontrolü geliştirilebilir
- Takvim görünümü (calendar view) eklenebilir
- Export işlemleri (PDF, Excel) eklenebilir
- Otomatik rapor gönderimi planlanabilir

### 4.4. Öğrenilen Konular

Bu proje sürecinde aşağıdaki konularda bilgi ve beceri kazanılmıştır:

1. **ASP.NET Core MVC:** Framework'ün temel yapısı, routing, middleware
2. **Entity Framework Core:** ORM kullanımı, migration, LINQ sorguları
3. **ASP.NET Core Identity:** Kimlik doğrulama ve yetkilendirme
4. **Dependency Injection:** Servis yaşam döngüleri ve DI kullanımı
5. **Razor View Engine:** Dynamic HTML üretimi, view components
6. **Bootstrap:** Responsive tasarım, grid system, components
7. **Git:** Versiyon kontrolü
8. **Debugging:** Hata ayıklama teknikleri
9. **Software Design Patterns:** MVC, Repository pattern konseptleri
10. **Database Design:** Normalizasyon, ilişkiler, indexleme

### 4.5. Gelecek Geliştirmeler

Sistemin gelecekte yapılabilecek geliştirmeleri:

1. **Bildirim Sistemi:**
   - E-posta bildirimleri (randevu onayı, reddi, hatırlatma)
   - SMS bildirimleri
   - Push notifications

2. **Takvim Entegrasyonu:**
   - FullCalendar.js ile takvim görünümü
   - Google Calendar, Outlook entegrasyonu
   - Randevu çakışma kontrolü

3. **Raporlama Geliştirmeleri:**
   - PDF ve Excel export
   - Otomatik haftalık/aylık raporlar
   - Daha detaylı analiz grafikleri

4. **Ödeme Sistemi:**
   - Online ödeme entegrasyonu
   - Randevu ücreti yönetimi
   - Fatura oluşturma

5. **Multi-Language Desteği:**
   - Türkçe/İngilizce dil seçeneği
   - Localization

6. **API Geliştirme:**
   - RESTful API
   - Mobil uygulama için backend

7. **Gelişmiş Güvenlik:**
   - Two-Factor Authentication (2FA)
   - OAuth/OpenID Connect
   - Rate limiting

8. **Performans İyileştirmeleri:**
   - Redis cache
   - Lazy loading optimizasyonu
   - Paging implementasyonu

### 4.6. Sonuç ve Değerlendirme

Online Randevu Yönetim Sistemi projesi, modern web geliştirme prensipleri ve teknolojileri kullanılarak başarıyla tamamlanmıştır. Sistem, gerçek dünya iş problemlerine çözüm üretmekte ve kullanıcılara değer katmaktadır.

Proje sürecinde edinilen deneyimler, yazılım mühendisliği prensiplerinin pratik uygulaması açısından son derece değerli olmuştur. MVC tasarım deseni, ORM kullanımı, güvenlik önlemleri ve kullanıcı deneyimi tasarımı gibi konularda derinlemesine bilgi edinilmiştir.

Sistemin modüler yapısı sayesinde gelecekte yeni özellikler kolayca eklenebilir. Proje, ölçeklenebilir ve bakımı kolay bir mimari üzerine inşa edilmiştir.

Sonuç olarak, bu proje hem akademik açıdan hem de pratik uygulamalar açısından hedeflerine ulaşmış, modern web geliştirme teknolojilerinin etkin kullanımına örnek teşkil etmiştir.

---

## 5. KAYNAKÇA

### 5.1. Resmi Dokümantasyonlar

[1] Microsoft. (2025). "ASP.NET Core Documentation". Microsoft Docs.  
https://docs.microsoft.com/en-us/aspnet/core/

[2] Microsoft. (2025). "Entity Framework Core Documentation". Microsoft Docs.  
https://docs.microsoft.com/en-us/ef/core/

[3] Microsoft. (2025). "ASP.NET Core Identity". Microsoft Docs.  
https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity

[4] Microsoft. (2025). "Model-View-Controller (MVC) Pattern". Microsoft Docs.  
https://docs.microsoft.com/en-us/aspnet/core/mvc/overview

[5] Microsoft. (2025). "Entity Framework Core - Code First Migrations". Microsoft Docs.  
https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/

### 5.2. Framework ve Kütüphane Kaynakları

[6] Bootstrap Team. (2025). "Bootstrap 5 Documentation".  
https://getbootstrap.com/docs/5.0/

[7] Chart.js Contributors. (2025). "Chart.js Documentation".  
https://www.chartjs.org/docs/

[8] Microsoft. (2025). "C# Programming Guide". Microsoft Docs.  
https://docs.microsoft.com/en-us/dotnet/csharp/

[9] Microsoft. (2025). "SQL Server Documentation". Microsoft Docs.  
https://docs.microsoft.com/en-us/sql/

### 5.3. Tasarım ve Mimari Kaynakları

[10] Fowler, M. (2002). "Patterns of Enterprise Application Architecture". Addison-Wesley.

[11] Microsoft. (2025). "Dependency Injection in ASP.NET Core". Microsoft Docs.  
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection

[12] Microsoft. (2025). "Security and Identity in ASP.NET Core". Microsoft Docs.  
https://docs.microsoft.com/en-us/aspnet/core/security/

[13] Microsoft. (2025). "Razor Pages with Entity Framework Core". Microsoft Docs.  
https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/

### 5.4. Web Geliştirme ve Best Practices

[14] Mozilla Developer Network. (2025). "HTML, CSS, JavaScript Documentation".  
https://developer.mozilla.org/

[15] W3C. (2025). "Web Content Accessibility Guidelines (WCAG)".  
https://www.w3.org/WAI/standards-guidelines/wcag/

[16] OWASP. (2025). "OWASP Top Ten Web Application Security Risks".  
https://owasp.org/www-project-top-ten/

### 5.5. Eğitim ve Tutorial Kaynakları

[17] Microsoft Learn. (2025). "Build web apps with ASP.NET Core for beginners".  
https://docs.microsoft.com/en-us/learn/paths/aspnet-core-web-app/

[18] Pluralsight. (2025). "ASP.NET Core Path".  
https://www.pluralsight.com/paths/aspnet-core

[19] Stack Overflow. (2025). "ASP.NET Core Questions and Answers".  
https://stackoverflow.com/questions/tagged/asp.net-core

### 5.6. Yazılım Geliştirme Araçları

[20] Microsoft. (2025). "Visual Studio 2022 Documentation".  
https://docs.microsoft.com/en-us/visualstudio/

[21] Git. (2025). "Git Documentation".  
https://git-scm.com/doc

[22] NuGet. (2025). "NuGet Documentation".  
https://docs.microsoft.com/en-us/nuget/

---

## EKLER

### EK-A: Kurulum Kılavuzu

**Gerekli Yazılımlar:**
1. .NET 8.0 SDK
2. Visual Studio 2022 veya Visual Studio Code
3. SQL Server LocalDB
4. Git (opsiyonel)

**Kurulum Adımları:**
1. Projeyi indirin veya klonlayın
2. `appsettings.json` dosyasında connection string'i kontrol edin
3. Terminal'de `dotnet ef database update` komutunu çalıştırın
4. `dotnet run` ile uygulamayı başlatın
5. Tarayıcıda `https://localhost:5001` adresine gidin

**İlk Giriş:**
- Email: admin@site.com
- Şifre: Admin123!

### EK-B: Kullanıcı Kılavuzu

**Müşteri İşlemleri:**
1. Register sayfasından kayıt olun
2. Admin'den Customer rolü atamasını isteyin
3. "Appointments" menüsünden randevu oluşturun
4. Randevularınızı listeleyin, düzenleyin veya silin

**Personel İşlemleri:**
1. Atanan randevuları görüntüleyin
2. "Change Status" ile durumu değiştirin
3. İsteğe bağlı personel notu ekleyin

**Admin İşlemleri:**
1. Users sayfasından kullanıcıları görüntüleyin
2. Manage Roles ile rol atayın
3. Report sayfasından raporları inceleyin

### EK-C: Proje Dosya Yapısı

```
AppointmentSystemFinal/
│
├── Controllers/
│   ├── HomeController.cs
│   ├── AppointmentsController.cs
│   └── AdminController.cs
│
├── Models/
│   ├── ApplicationUser.cs
│   ├── Appointment.cs
│   └── ErrorViewModel.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Views/
│   ├── Home/
│   ├── Appointments/
│   ├── Admin/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
│
├── Migrations/
├── Areas/Identity/
├── appsettings.json
├── Program.cs
└── README.md
```

### EK-D: Veritabanı Script'leri

**Roller Oluşturma:**
```sql
INSERT INTO AspNetRoles (Id, Name, NormalizedName)
VALUES 
  (NEWID(), 'Admin', 'ADMIN'),
  (NEWID(), 'Staff', 'STAFF'),
  (NEWID(), 'Customer', 'CUSTOMER');
```

**Örnek Randevu:**
```sql
INSERT INTO Appointments (CustomerId, StaffId, Date, StartTime, EndTime, Title, Status, CreatedAt)
VALUES ('user-id', 'staff-id', '2025-11-20', '09:00:00', '10:00:00', 'Genel Muayene', 'Pending', GETDATE());
```

---

**Proje Teslim Tarihi:** 17 Kasım 2025  
**Proje Durumu:** Tamamlandı ✓

**İmza:**  
_________________  
Ferhat Bayutmus

