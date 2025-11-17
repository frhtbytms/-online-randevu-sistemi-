// =============================================================================
// AdminController: Yönetici İşlemleri Controller'ı
// AÇIKLAMA: Kullanıcı yönetimi, rol atama ve raporlama işlemleri
// YETKİLENDİRME: Sadece Admin rolü erişebilir
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppointmentSystemFinal.Data;
using AppointmentSystemFinal.Models;

namespace AppointmentSystemFinal.Controllers
{
    /// <summary>
    /// Yönetici işlemlerini yöneten controller
    /// [Authorize(Roles = "Admin")]: Sadece Admin rolündeki kullanıcılar erişebilir
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Dependency Injection ile gelen servisler
        private readonly ApplicationDbContext _context;           // Veritabanı bağlamı
        private readonly UserManager<ApplicationUser> _userManager;    // Kullanıcı yöneticisi
        private readonly RoleManager<IdentityRole> _roleManager;       // Rol yöneticisi

        /// <summary>
        /// Constructor: Servisleri enjekte eder
        /// </summary>
        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ========== KULLANICI LİSTESİ ==========
        /// <summary>
        /// Sistemdeki tüm kullanıcıları ve rollerini listeler
        /// </summary>
        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            // Tüm kullanıcıları al
            var users = await _userManager.Users.ToListAsync();
            
            // Her kullanıcı için rolleri al ve ViewModel'e dönüştür
            var userViewModels = new List<UserWithRolesViewModel>();
            foreach (var user in users)
            {
                // Kullanıcının rollerini al
                var roles = await _userManager.GetRolesAsync(user);
                
                // ViewModel'e ekle
                userViewModels.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // ========== KULLANICI ROL YÖNETİMİ ==========
        /// <summary>
        /// Belirli bir kullanıcının rollerini yönetme sayfasını gösterir
        /// </summary>
        // GET: Admin/ManageRoles/userId
        public async Task<IActionResult> ManageRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var viewModel = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? "",
                FullName = user.FullName,
                Email = user.Email ?? "",
                UserRoles = userRoles.ToList(),
                AllRoles = allRoles.Select(r => r.Name ?? "").ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Mevcut rolleri al
            var userRoles = await _userManager.GetRolesAsync(user);

            // Tüm rolleri kaldır
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!removeResult.Succeeded)
            {
                TempData["Error"] = "Roller kaldırılırken bir hata oluştu.";
                return RedirectToAction(nameof(Users));
            }

            // Yeni rolleri ekle
            if (selectedRoles != null && selectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    TempData["Error"] = "Roller eklenirken bir hata oluştu.";
                    return RedirectToAction(nameof(Users));
                }
            }

            TempData["Success"] = $"{user.FullName} kullanıcısının rolleri başarıyla güncellendi.";
            return RedirectToAction(nameof(Users));
        }

        // ========== KULLANICI SİLME ==========
        // GET: Admin/DeleteUser/userId
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kendi hesabını silmeye çalışıyor mu?
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Users));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserWithRolesViewModel
            {
                User = user,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/DeleteUser
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kendi hesabını silmeye çalışıyor mu?
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"{user.FullName} kullanıcısı başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Users));
        }

        // ========== RAPORLAMA ==========
        // GET: Admin/Report
        public async Task<IActionResult> Report()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);

            var report = new AppointmentReportViewModel
            {
                // Toplam istatistikler
                TotalAppointments = await _context.Appointments.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalCustomers = (await _userManager.GetUsersInRoleAsync("Customer")).Count,
                TotalStaff = (await _userManager.GetUsersInRoleAsync("Staff")).Count,

                // Durum bazlı sayılar
                PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending"),
                ApprovedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Approved"),
                RejectedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Rejected"),
                CancelledAppointments = await _context.Appointments.CountAsync(a => a.Status == "Cancelled"),

                // Bu hafta
                ThisWeekAppointments = await _context.Appointments
                    .CountAsync(a => a.Date >= thisWeekStart),
                ThisWeekApproved = await _context.Appointments
                    .CountAsync(a => a.Date >= thisWeekStart && a.Status == "Approved"),
                ThisWeekRejected = await _context.Appointments
                    .CountAsync(a => a.Date >= thisWeekStart && a.Status == "Rejected"),

                // Bu ay
                ThisMonthAppointments = await _context.Appointments
                    .CountAsync(a => a.Date >= thisMonthStart),
                ThisMonthApproved = await _context.Appointments
                    .CountAsync(a => a.Date >= thisMonthStart && a.Status == "Approved"),
                ThisMonthRejected = await _context.Appointments
                    .CountAsync(a => a.Date >= thisMonthStart && a.Status == "Rejected"),

                // Bugün
                TodayAppointments = await _context.Appointments
                    .CountAsync(a => a.Date == today),

                // Son 7 gün randevu sayıları (grafik için)
                Last7DaysData = await GetLast7DaysAppointmentData()
            };

            return View(report);
        }

        // Son 7 günün randevu verilerini al
        private async Task<Dictionary<string, int>> GetLast7DaysAppointmentData()
        {
            var data = new Dictionary<string, int>();
            var today = DateTime.Today;

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = await _context.Appointments
                    .CountAsync(a => a.Date == date);
                data[date.ToString("dd.MM")] = count;
            }

            return data;
        }

        // ========== EN AKTİF PERSONEL RAPORU ==========
        // GET: Admin/StaffReport
        public async Task<IActionResult> StaffReport()
        {
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            var staffReports = new List<StaffReportViewModel>();

            foreach (var staff in staffUsers)
            {
                var totalAppointments = await _context.Appointments
                    .CountAsync(a => a.StaffId == staff.Id);
                var approvedAppointments = await _context.Appointments
                    .CountAsync(a => a.StaffId == staff.Id && a.Status == "Approved");
                var rejectedAppointments = await _context.Appointments
                    .CountAsync(a => a.StaffId == staff.Id && a.Status == "Rejected");
                var pendingAppointments = await _context.Appointments
                    .CountAsync(a => a.StaffId == staff.Id && a.Status == "Pending");

                staffReports.Add(new StaffReportViewModel
                {
                    StaffName = staff.FullName,
                    StaffEmail = staff.Email ?? "",
                    TotalAppointments = totalAppointments,
                    ApprovedAppointments = approvedAppointments,
                    RejectedAppointments = rejectedAppointments,
                    PendingAppointments = pendingAppointments
                });
            }

            return View(staffReports.OrderByDescending(s => s.TotalAppointments).ToList());
        }
    }

    // ========== VIEW MODELS ==========

    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; } = new ApplicationUser();
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
    }

    public class AppointmentReportViewModel
    {
        public int TotalAppointments { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalStaff { get; set; }

        public int PendingAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int RejectedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        public int ThisWeekAppointments { get; set; }
        public int ThisWeekApproved { get; set; }
        public int ThisWeekRejected { get; set; }

        public int ThisMonthAppointments { get; set; }
        public int ThisMonthApproved { get; set; }
        public int ThisMonthRejected { get; set; }

        public int TodayAppointments { get; set; }

        public Dictionary<string, int> Last7DaysData { get; set; } = new Dictionary<string, int>();
    }

    public class StaffReportViewModel
    {
        public string StaffName { get; set; } = string.Empty;
        public string StaffEmail { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int RejectedAppointments { get; set; }
        public int PendingAppointments { get; set; }
    }
}
