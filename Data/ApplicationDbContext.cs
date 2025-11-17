// =============================================================================
// ApplicationDbContext: Veritabanı Bağlam Sınıfı
// AÇIKLAMA: Entity Framework Core ile veritabanı işlemlerini yöneten sınıf
// MİRAS: IdentityDbContext<ApplicationUser> - Identity tabloları dahil
// =============================================================================

using AppointmentSystemFinal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystemFinal.Data
{
    /// <summary>
    /// Entity Framework Core DbContext sınıfı.
    /// Identity tabloları ve uygulama tablolarını yönetir.
    /// IdentityDbContext'ten türer, böylece kullanıcı/rol tabloları otomatik oluşur.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor: DbContext yapılandırma seçeneklerini alır
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ========== DbSet TANIMLAMALARı ==========
        // Her DbSet bir veritabanı tablosunu temsil eder
        
        /// <summary>
        /// Appointments tablosu - Randevuları tutar
        /// </summary>
        public DbSet<Appointment> Appointments { get; set; }

        // ========== MODEL OLUŞTURMA ve İLİŞKİLER ==========
        /// <summary>
        /// Model oluşturma aşamasında çağrılır.
        /// Tablo ilişkilerini, index'leri ve kuralları burada tanımlarız.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Base sınıfın (Identity) konfigürasyonlarını uygula
            base.OnModelCreating(builder);

            // ========== APPOINTMENT - CUSTOMER İLİŞKİSİ ==========
            // Her randevunun bir müşterisi var (1:N ilişki)
            builder.Entity<Appointment>()
                .HasOne(a => a.Customer)                      // Bir randevunun bir Customer'ı var
                .WithMany(u => u.CustomerAppointments)        // Bir Customer'ın birden fazla randevusu var
                .HasForeignKey(a => a.CustomerId)             // Foreign Key: CustomerId
                .OnDelete(DeleteBehavior.Restrict);           // Müşteri silinirse randevu silinmesin

            // ========== APPOINTMENT - STAFF İLİŞKİSİ ==========
            // Her randevuya bir personel atanabilir (1:N ilişki, opsiyonel)
            builder.Entity<Appointment>()
                .HasOne(a => a.Staff)                         // Bir randevunun bir Staff'ı var (nullable)
                .WithMany(u => u.StaffAppointments)           // Bir Staff'ın birden fazla randevusu var
                .HasForeignKey(a => a.StaffId)                // Foreign Key: StaffId
                .OnDelete(DeleteBehavior.SetNull);            // Personel silinirse StaffId null olsun

            // ========== INDEXES (PERFORMANS İÇİN) ==========
            // Index'ler veritabanı sorgularını hızlandırır
            
            // CustomerId üzerinde index - Müşterinin randevularını hızlı bulmak için
            builder.Entity<Appointment>()
                .HasIndex(a => a.CustomerId);

            // StaffId üzerinde index - Personelin randevularını hızlı bulmak için
            builder.Entity<Appointment>()
                .HasIndex(a => a.StaffId);

            // Date üzerinde index - Tarihe göre filtreleme için
            builder.Entity<Appointment>()
                .HasIndex(a => a.Date);

            // Status üzerinde index - Duruma göre filtreleme için
            builder.Entity<Appointment>()
                .HasIndex(a => a.Status);

            // ========== DEFAULT DEĞERLER ==========
            // Veritabanı seviyesinde varsayılan değerler
            
            // Randevu durumu varsayılan olarak "Pending" (Beklemede)
            builder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasDefaultValue("Pending");

            // Oluşturulma tarihi varsayılan olarak şimdiki zaman (SQL Server fonksiyonu)
            builder.Entity<Appointment>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
