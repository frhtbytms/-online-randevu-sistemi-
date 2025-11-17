# Proje Kod Açıklamaları

Bu döküman projedeki kodların ne işe yaradığını açıklar.

---

## Proje Yapısı

```
AppointmentSystemFinal/
│
├── Program.cs                    -> Uygulama başlangıcı
├── appsettings.json              -> Ayarlar (veritabanı bağlantısı)
│
├── Controllers/                  -> Controller dosyaları
│   ├── HomeController.cs
│   ├── AppointmentsController.cs -> Randevu işlemleri
│   └── AdminController.cs        -> Yönetici işlemleri
│
├── Models/                       -> Model sınıfları (veri yapıları)
│   ├── ApplicationUser.cs        -> Kullanıcı modeli
│   └── Appointment.cs            -> Randevu modeli
│
├── Data/
│   └── ApplicationDbContext.cs   -> Veritabanı bağlantısı
│
├── Views/                        -> HTML sayfaları
│   ├── Home/
│   ├── Appointments/
│   └── Admin/
│
└── wwwroot/                      -> CSS, JS dosyaları
```

---

## Önemli Dosyalar

### 1. Program.cs
Uygulamanın başlangıç dosyası. Burada şunlar yapılıyor:

- Veritabanı bağlantısı kuruluyor
- Identity sistemi ayarlanıyor (kullanıcı girişi için)
- Roller oluşturuluyor (Admin, Staff, Customer)
- İlk admin kullanıcısı yaratılıyor

**İlk Admin:**
- Email: admin@site.com
- Şifre: Admin123!

### 2. ApplicationUser.cs
Kullanıcı bilgilerini tutar. Normal Identity User'a ek olarak:
- FirstName (Ad)
- LastName (Soyad)
- FullName (Ad + Soyad)

### 3. Appointment.cs
Randevu bilgilerini tutar:

**Alanlar:**
- CustomerId: Müşteri
- StaffId: Personel (atanabilir)
- Date: Randevu tarihi
- StartTime, EndTime: Saat aralığı
- Title: Randevu başlığı
- Description: Açıklama
- Status: Durum (Pending/Approved/Rejected/Cancelled)
- StaffNote: Personel notu

**Durumlar:**
- Pending = Beklemede
- Approved = Onaylandı
- Rejected = Reddedildi
- Cancelled = İptal Edildi

### 4. ApplicationDbContext.cs
Veritabanıyla konuşan sınıf. Entity Framework kullanıyor.

**İlişkiler:**
- Müşteri -> Randevular (1:N)
- Personel -> Randevular (1:N)

### 5. AppointmentsController.cs
Randevu işlemlerini yapan controller.

**Metodlar:**

**Index()** - Randevu Listesi
- Admin: Hepsini görür
- Staff: Kendine atananları görür
- Customer: Sadece kendin görür

**Create()** - Yeni Randevu
- Sadece Customer kullanabilir
- Tarih geçmişte olamaz
- Bitiş saati > Başlangıç saati

**Edit()** - Düzenleme
- Customer: Kendi randevusunu düzenler
- Admin: Hepsini düzenler

**Delete()** - Silme
- Customer: Kendi randevusunu siler
- Admin: Hepsini siler

**ChangeStatus()** - Durum Değiştirme
- Staff: Kendi randevularının durumunu değiştirir
- Admin: Hepsini değiştirir

### 6. AdminController.cs
Yönetici işlemleri.

**Metodlar:**

**Users()** - Kullanıcı Listesi
Tüm kullanıcıları ve rollerini gösterir.

**ManageRoles()** - Rol Atama
Kullanıcılara rol atar/kaldırır.

**DeleteUser()** - Kullanıcı Silme
Kullanıcıyı siler (kendi hesabını silemez).

**Report()** - Raporlar
İstatistikler ve grafikler gösterir:
- Toplam randevu sayısı
- Durumlara göre sayılar
- Son 7 gün grafiği

**StaffReport()** - Personel Raporu
Her personelin performansını gösterir.

---

## Güvenlik

**Authorize Attribute:**
```csharp
[Authorize(Roles = "Admin")]  -> Sadece Admin
[Authorize]                   -> Giriş yapmış herkes
```

**Manuel Kontroller:**
```csharp
if (!User.IsInRole("Admin") && appointment.CustomerId != currentUser.Id) {
    return Forbid();  // İzin yok
}
```

---

## Veritabanı

**Tablolar:**
- AspNetUsers (Kullanıcılar)
- AspNetRoles (Roller)
- AspNetUserRoles (Kullanıcı-Rol bağlantısı)
- Appointments (Randevular)

**İlişkiler:**
```
AspNetUsers ─┬─> Appointments (Customer olarak)
             └─> Appointments (Staff olarak)
```

---

## CRUD Nedir?

- **C**reate: Yeni kayıt ekle
- **R**ead: Kayıtları oku/göster
- **U**pdate: Kaydı güncelle
- **D**elete: Kaydı sil

Randevular için:
- Create: `AppointmentsController.Create()`
- Read: `AppointmentsController.Index()`, `Details()`
- Update: `AppointmentsController.Edit()`
- Delete: `AppointmentsController.Delete()`

---

## Örnek Akış: Müşteri Randevu Alıyor

1. Müşteri login olur
2. "Yeni Randevu" butonuna tıklar
3. Form açılır (tarih, saat, başlık vs.)
4. Formu doldurur, gönderir
5. Controller validasyon yapar
6. Veritabanına kaydeder
7. Liste sayfasına yönlendirir
8. "Randevu oluşturuldu" mesajı gösterir

---

## Önemli Kavramlar

**Dependency Injection:**
Constructor'da servisleri alıyoruz:
```csharp
public AppointmentsController(ApplicationDbContext context, ...)
{
    _context = context;
}
```

**Include (Entity Framework):**
İlişkili verileri birlikte çek:
```csharp
_context.Appointments
    .Include(a => a.Customer)  // Müşteriyi de getir
    .Include(a => a.Staff)     // Personeli de getir
```

**Async/Await:**
Beklemeli işlemler (veritabanı sorguları):
```csharp
var data = await _context.Appointments.ToListAsync();
```

**TempData:**
Bir sonraki sayfada mesaj göster:
```csharp
TempData["Success"] = "İşlem başarılı!";
```

---

## Kullanılan Teknolojiler

- ASP.NET Core 8.0 MVC
- Entity Framework Core (Code First)
- SQL Server LocalDB
- ASP.NET Core Identity
- Bootstrap 5
- Chart.js (grafikler için)

---

**Tarih:** 17 Kasım 2025
**Proje:** Online Randevu Yönetim Sistemi
