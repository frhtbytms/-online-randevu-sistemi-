// ApplicationUser.cs
// Kullanici modeli - Identity'den gelir

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppointmentSystemFinal.Models
{
    // IdentityUser'i genisletiyoruz (Ad ve Soyad eklemek icin)
    public class ApplicationUser : IdentityUser
    {
        // Kullanici Adi
        [Required(ErrorMessage = "Ad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        // Kullanici Soyadi
        [Required(ErrorMessage = "Soyad alanı zorunludur")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        // Ad Soyad birlesik hali
        [Display(Name = "Ad Soyad")]
        public string FullName => $"{FirstName} {LastName}";

        // Musteri olarak yapilan randevular
        public virtual ICollection<Appointment> CustomerAppointments { get; set; } = new List<Appointment>();

        // Personel olarak atanan randevular
        public virtual ICollection<Appointment> StaffAppointments { get; set; } = new List<Appointment>();
    }
}
