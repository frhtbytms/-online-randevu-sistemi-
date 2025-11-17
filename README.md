# Online Randevu Yönetim Sistemi

ASP.NET Core MVC ile yapılmış randevu sistemi projesi.

## Kullanılan Teknolojiler

- ASP.NET Core 8.0 MVC
- Entity Framework Core (Code First)
- SQL Server LocalDB
- ASP.NET Core Identity
- Bootstrap 5
- C#

## Özellikler

### Roller (3 tane)

**Admin:** Herşeyi görebilir ve yönetebilir
- Kullanıcıları görüntüler
- Rol atar/kaldırır
- Tüm randevuları görür
- Raporlar gösterir

**Staff (Personel):** Randevuları yönetir
- Atanan randevuları görür
- Durum değiştirir (Onayla/Reddet)
- Not ekler

**Customer (Müşteri):** Randevu alır
- Yeni randevu oluşturur
- Kendi randevularını görür
- Düzenler ve iptal eder

## Veritabanı

**AspNetUsers** - Kullanıcılar (Ad, Soyad eklendi)
**Appointments** - Randevular

İlişkiler:
- Müşteri -> Randevular (1:N)
- Personel -> Randevular (1:N)

## Kurulum

### Gereklilikler
- .NET 8.0 SDK
- Visual Studio 2022 veya VS Code
- SQL Server LocalDB

### Adımlar

1. Projeyi aç:
```bash
cd AppointmentSystemFinal
```

2. appsettings.json'daki connection string'i kontrol et:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AppointmentSystemDb;..."
```

3. Migration'ları uygula:
```bash
dotnet ef database update
```

4. Uygulamayı çalıştır:
```bash
dotnet run
```

5. Tarayıcıda aç: https://localhost:5001

## İlk Giriş

**Admin Hesabı:**
- Email: admin@site.com
- Şifre: Admin123!

Admin giriş yaptıktan sonra:
1. Users'a git
2. Kullanıcı seç
3. Manage Roles'dan rol ata

## Kullanım

### Müşteri Olarak

1. Register'dan kayıt ol
2. Admin'e rol vermesini söyle (veya kendin admin ile ata)
3. "Appointments" menüsüne tıkla
4. "Create New" ile randevu oluştur
5. Tarih, saat, başlık gir
6. Personel seç
7. Kaydet

### Personel Olarak

1. "Appointments" menüsüne tıkla
2. Sana atanan randevuları görürsün
3. "Change Status" butonuna tıkla
4. Durum seç (Approve/Reject)
5. İsteğe bağlı not ekle
6. Kaydet

### Admin Olarak

1. "Admin" menüsünden işlemler yapabilirsin:
   - Users: Kullanıcı listesi
   - Manage Roles: Rol atama
   - Delete: Kullanıcı silme
   - Report: Genel rapor
   - Staff Report: Personel raporu

## Randevu Durumları

- **Pending** (Beklemede): Yeni oluşturulmuş
- **Approved** (Onaylandı): Personel onayladı
- **Rejected** (Reddedildi): Personel reddetti
- **Cancelled** (İptal): Müşteri iptal etti

## Proje Yapısı

```
Controllers/
  - HomeController.cs (Ana sayfa)
  - AppointmentsController.cs (Randevu işlemleri)
  - AdminController.cs (Yönetici işlemleri)

Models/
  - ApplicationUser.cs (Kullanıcı)
  - Appointment.cs (Randevu)

Data/
  - ApplicationDbContext.cs (Veritabanı)

Views/
  - Home/ (Ana sayfa viewları)
  - Appointments/ (Randevu viewları)
  - Admin/ (Admin viewları)
```

## Önemli Notlar

- İlk çalıştırmada migration otomatik uygulanır
- Admin, Staff, Customer rolleri otomatik oluşur
- admin@site.com kullanıcısı otomatik oluşur
- Müşteri sadece kendi randevularını görebilir
- Personel sadece kendine atanan randevuları görebilir
- Admin herşeyi görebilir

## Test Senaryoları

### Senaryo 1: Müşteri Randevu Alıyor
1. Customer hesabıyla giriş yap
2. Appointments > Create New
3. Form doldur
4. Kaydet
5. Index'te görünmelidir

### Senaryo 2: Personel Onaylıyor
1. Staff hesabıyla giriş yap
2. Kendine atanan randevuyu gör
3. Change Status'a tıkla
4. "Approved" seç
5. Not ekle
6. Kaydet

### Senaryo 3: Admin Rol Atıyor
1. Admin ile giriş yap
2. Admin > Users
3. Kullanıcı seç
4. Manage Roles
5. Staff checkbox işaretle
6. Assign Roles

## Yazar

Ferhat - İnternet Programcılığı Final Projesi

## Tarih

17 Kasım 2025
