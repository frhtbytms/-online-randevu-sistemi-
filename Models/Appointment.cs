// Appointment.cs
// Randevu modeli

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentSystemFinal.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // Musteri bilgisi
        [Required(ErrorMessage = "Müşteri bilgisi zorunludur")]
        [Display(Name = "Müşteri ID")]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey(nameof(CustomerId))]
        [Display(Name = "Müşteri")]
        public virtual ApplicationUser? Customer { get; set; }

        // Personel bilgisi (bos olabilir)
        [Display(Name = "Atanan Personel ID")]
        public string? StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        [Display(Name = "Atanan Personel")]
        public virtual ApplicationUser? Staff { get; set; }

        // Randevu tarihi
        [Required(ErrorMessage = "Randevu tarihi zorunludur")]
        [DataType(DataType.Date)]
        [Display(Name = "Randevu Tarihi")]
        public DateTime Date { get; set; }

        // Baslangic saati
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [DataType(DataType.Time)]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }

        // Bitis saati
        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        [DataType(DataType.Time)]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; }

        // Randevu basligi
        [Required(ErrorMessage = "Randevu başlığı zorunludur")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        [Display(Name = "Randevu Başlığı")]
        public string Title { get; set; } = string.Empty;

        // Aciklama (opsiyonel)
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // Randevu durumu (Pending, Approved, Rejected, Cancelled)
        [Required]
        [StringLength(50)]
        [Display(Name = "Durum")]
        public string Status { get; set; } = "Pending";

        // Personel notu
        [StringLength(500, ErrorMessage = "Personel notu en fazla 500 karakter olabilir")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Personel Notu")]
        public string? StaffNote { get; set; }

        // Olusturma tarihi
        [Required]
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Guncellenme tarihi
        [Display(Name = "Güncellenme Tarihi")]
        public DateTime? UpdatedAt { get; set; }

        // Durum Turkce karsiligi
        [Display(Name = "Durum Açıklaması")]
        public string StatusDisplay
        {
            get
            {
                return Status switch
                {
                    "Pending" => "Beklemede",
                    "Approved" => "Onaylandı",
                    "Rejected" => "Reddedildi",
                    "Cancelled" => "İptal Edildi",
                    _ => Status
                };
            }
        }

        // Bootstrap renk sinifi
        [Display(Name = "Durum Rengi")]
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
    }
}
