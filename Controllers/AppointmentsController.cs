// =============================================================================
// AppointmentsController: Randevu İşlemleri Controller'ı
// AÇIKLAMA: Randevuların CRUD işlemlerini ve rol bazlı yetkilendirmeyi yönetir
// YETKİLENDİRME: Admin, Staff ve Customer rollerine göre farklı işlemler
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppointmentSystemFinal.Data;
using AppointmentSystemFinal.Models;

namespace AppointmentSystemFinal.Controllers
{
    /// <summary>
    /// Randevu işlemlerini yöneten controller
    /// [Authorize]: Sadece giriş yapmış kullanıcılar erişebilir
    /// </summary>
    [Authorize]
    public class AppointmentsController : Controller
    {
        // Dependency Injection ile gelen servisler
        private readonly ApplicationDbContext _context;      // Veritabanı bağlamı
        private readonly UserManager<ApplicationUser> _userManager;  // Kullanıcı yöneticisi

        /// <summary>
        /// Constructor: Servisleri enjekte eder
        /// </summary>
        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ========== INDEX - Randevu Listesi ==========
        /// <summary>
        /// Randevu listesini gösterir
        /// ROL BAZLI FİLTRELEME:
        /// - Admin: Tüm randevuları görür
        /// - Staff: Sadece kendisine atanan randevuları görür
        /// - Customer: Sadece kendi oluşturduğu randevuları görür
        /// </summary>
        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            // Şu anda giriş yapmış kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // Kullanıcı bulunamazsa, giriş sayfasına yönlendir
                return Challenge();
            }

            // Tüm randevuları sorgula (henüz filtrelenmemiş)
            // Include: İlişkili tabloları da getir (Customer ve Staff bilgileri)
            IQueryable<Appointment> appointmentsQuery = _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff);

            // Rol bazlı filtreleme
            if (User.IsInRole("Admin"))
            {
                // ADMIN: Tüm randevuları görebilir, filtreleme yapma
                ViewData["Title"] = "Tüm Randevular";
                ViewData["UserRole"] = "Admin";
            }
            else if (User.IsInRole("Staff"))
            {
                // STAFF: Sadece kendisine atanan randevuları göster
                appointmentsQuery = appointmentsQuery.Where(a => a.StaffId == currentUser.Id);
                ViewData["Title"] = "Atandığım Randevular";
                ViewData["UserRole"] = "Staff";
            }
            else // Customer
            {
                // CUSTOMER: Sadece kendi oluşturduğu randevuları göster
                appointmentsQuery = appointmentsQuery.Where(a => a.CustomerId == currentUser.Id);
                ViewData["Title"] = "Randevularım";
                ViewData["UserRole"] = "Customer";
            }

            // Randevuları sırala: Önce tarihe göre (yeniden eskiye), sonra saate göre
            var appointments = await appointmentsQuery
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();

            // View'a gönder
            return View(appointments);
        }

        // ========== DETAILS - Randevu Detayı ==========
        /// <summary>
        /// Belirli bir randevunun detaylarını gösterir
        /// YETKİ KONTROLÜ: Admin, randevu sahibi veya atanan personel görebilir
        /// </summary>
        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // ID parametresi boş mu kontrol et
            if (id == null)
            {
                return NotFound();
            }

            // Veritabanından randevuyu al (müşteri ve personel bilgileriyle birlikte)
            var appointment = await _context.Appointments
                .Include(a => a.Customer)    // İlişkili müşteri bilgisi
                .Include(a => a.Staff)       // İlişkili personel bilgisi
                .FirstOrDefaultAsync(m => m.Id == id);

            // Randevu bulunamadıysa 404 döndür
            if (appointment == null)
            {
                return NotFound();
            }

            // YETKİ KONTROLÜ: Sadece yetkili kullanıcılar görebilir
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Admin DEĞİLSE ve randevu sahibi DEĞİLSE ve atanan personel DEĞİLSE
            if (!User.IsInRole("Admin") && 
                appointment.CustomerId != currentUser.Id && 
                appointment.StaffId != currentUser.Id)
            {
                // Erişim reddedildi (403 Forbidden)
                return Forbid();
            }

            // Her şey OK, detayları göster
            return View(appointment);
        }

        // ========== CREATE - Randevu Oluşturma ==========
        /// <summary>
        /// Yeni randevu oluşturma formunu gösterir
        /// YETKİLENDİRME: Sadece Customer rolü randevu oluşturabilir
        /// </summary>
        // GET: Appointments/Create
        [Authorize(Roles = "Customer")]  // Sadece müşteriler bu sayfaya erişebilir
        public async Task<IActionResult> Create()
        {
            // Personel listesini al (dropdown için)
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            // SelectList: HTML select dropdown için veri hazırla
            ViewData["StaffId"] = new SelectList(staffUsers, "Id", "FullName");
            
            // Varsayılan değerlerle model oluştur
            var model = new Appointment
            {
                Date = DateTime.Today.AddDays(1),    // Yarın
                StartTime = new TimeSpan(9, 0, 0),   // Saat 09:00
                EndTime = new TimeSpan(10, 0, 0)     // Saat 10:00
            };

            return View(model);
        }

        /// <summary>
        /// Yeni randevu oluşturma işlemini gerçekleştirir (Form POST)
        /// VALİDASYON: Tarih ve saat kontrolleri yapılır
        /// </summary>
        // POST: Appointments/Create
        [HttpPost]                           // Sadece POST istekleri
        [ValidateAntiForgeryToken]           // CSRF saldırılarına karşı koruma
        [Authorize(Roles = "Customer")]      // Sadece müşteriler
        public async Task<IActionResult> Create([Bind("Title,Description,Date,StartTime,EndTime,StaffId")] Appointment appointment)
        {
            // Şu anki kullanıcıyı al
            var currentUser = await _userManager.GetUserAsync(User);
            
            // Otomatik alanları doldur
            appointment.CustomerId = currentUser.Id;    // Müşteri ID'sini ata
            appointment.Status = "Pending";              // Durum: Beklemede
            appointment.CreatedAt = DateTime.Now;        // Oluşturulma zamanı

            // === MANUEL VALİDASYON KONTROLLARI ===
            
            // 1. Tarih kontrolü: Geçmiş tarih olamaz
            if (appointment.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Randevu tarihi bugünden önceki bir tarih olamaz.");
            }

            // 2. Saat kontrolü: Bitiş > Başlangıç olmalı
            if (appointment.EndTime <= appointment.StartTime)
            {
                ModelState.AddModelError("EndTime", "Bitiş saati başlangıç saatinden büyük olmalıdır.");
            }

            // Model validasyonu geçti mi?
            if (ModelState.IsValid)
            {
                // Veritabanına ekle
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                
                // Başarı mesajı göster (TempData: Bir sonraki request'te görünür)
                TempData["Success"] = "Randevu başarıyla oluşturuldu.";
                
                // Randevu listesi sayfasına yönlendir
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa, formu tekrar göster
            // Staff listesini tekrar doldur (form yeniden gösterilecek)
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            ViewData["StaffId"] = new SelectList(staffUsers, "Id", "FullName", appointment.StaffId);
            
            return View(appointment);
        }

        // ========== EDIT - Randevu Düzenleme ==========
        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Yetki kontrolü
            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && appointment.CustomerId != currentUser.Id)
            {
                return Forbid();
            }

            // Staff listesi (Admin için)
            if (User.IsInRole("Admin"))
            {
                var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
                ViewData["StaffId"] = new SelectList(staffUsers, "Id", "FullName", appointment.StaffId);
            }

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,StaffId,Title,Description,Date,StartTime,EndTime,Status,StaffNote,CreatedAt")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            // Yetki kontrolü
            var currentUser = await _userManager.GetUserAsync(User);
            var originalAppointment = await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            
            if (originalAppointment == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && originalAppointment.CustomerId != currentUser.Id)
            {
                return Forbid();
            }

            // Validasyon
            if (appointment.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Randevu tarihi bugünden önceki bir tarih olamaz.");
            }

            if (appointment.EndTime <= appointment.StartTime)
            {
                ModelState.AddModelError("EndTime", "Bitiş saati başlangıç saatinden büyük olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.UpdatedAt = DateTime.Now;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Randevu başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Hata durumunda Staff listesini tekrar doldur
            if (User.IsInRole("Admin"))
            {
                var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
                ViewData["StaffId"] = new SelectList(staffUsers, "Id", "FullName", appointment.StaffId);
            }

            return View(appointment);
        }

        // ========== DELETE - Randevu Silme ==========
        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Yetki kontrolü
            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && appointment.CustomerId != currentUser.Id)
            {
                return Forbid();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Yetki kontrolü
            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && appointment.CustomerId != currentUser.Id)
            {
                return Forbid();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Randevu başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        // ========== CHANGE STATUS - Durum Değiştirme (Staff/Admin) ==========
        // GET: Appointments/ChangeStatus/5
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Staff ise sadece kendisine atananları değiştirebilir
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Staff") && appointment.StaffId != currentUser.Id)
            {
                return Forbid();
            }

            return View(appointment);
        }

        // POST: Appointments/ChangeStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> ChangeStatus(int id, string status, string staffNote)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Staff ise sadece kendisine atananları değiştirebilir
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Staff") && appointment.StaffId != currentUser.Id)
            {
                return Forbid();
            }

            appointment.Status = status;
            appointment.StaffNote = staffNote;
            appointment.UpdatedAt = DateTime.Now;

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevu durumu başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
