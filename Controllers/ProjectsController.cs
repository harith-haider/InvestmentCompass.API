using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentCompass.API.Data;
using InvestmentCompass.API.Models;

namespace InvestmentCompass.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. مسار لجلب المشاريع المعلقة (ستستخدمه صفحة الإدارة VerificationQueue)
        // GET: api/projects/pending
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Project>>> GetPendingProjects()
        {
            var pendingProjects = await _context.Projects
                .Include(p => p.Owner) // هذا السطر ضروري لإظهار اسم صاحب المشروع في واجهة الإدارة
                .Where(p => p.Status == "Pending")
                .ToListAsync();

            return Ok(pendingProjects);
        }

        // 2. مسار لإضافة مشروع جديد (ستستخدمه صفحة SubmitProject)
        // POST: api/projects
        [HttpPost]
        public async Task<ActionResult<Project>> SubmitProject(Project project)
        {
            // تحديد وقت التقديم تلقائياً
            project.SubmittedAt = DateTime.UtcNow;
            
            // إضافة المشروع لقاعدة البيانات وحفظ التغييرات
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        // 3. مسار لتحديث حالة المشروع (عبر الرابط مباشرة لتجنب أخطاء JSON)
        // PUT: api/projects/5/status/Approved
        [HttpPut("{id}/status/{newStatus}")]
        public async Task<IActionResult> UpdateProjectStatus(int id, string newStatus)
        {
            // البحث عن المشروع في قاعدة البيانات باستخدام الـ ID
            var project = await _context.Projects.FindAsync(id);
            
            if (project == null)
            {
                return NotFound();
            }

            // تحقق أمني: التأكد من أن القيمة المرسلة صحيحة فقط
            if (newStatus != "Approved" && newStatus != "Rejected")
            {
                return BadRequest("Invalid status.");
            }

            // تحديث الحالة وحفظ التغييرات
            project.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Project successfully updated to {newStatus}" });
        }
        // 4. مسار لجلب المشاريع المقبولة (للمستثمرين في الصفحة الرئيسية)
        // GET: api/projects/approved
        [HttpGet("approved")]
        public async Task<ActionResult<IEnumerable<Project>>> GetApprovedProjects()
        {
            var approvedProjects = await _context.Projects
                .Include(p => p.Owner) // لجلب بيانات صاحب المشروع
                .Where(p => p.Status == "Approved") // جلب المقبولة فقط
                .ToListAsync();

            return Ok(approvedProjects);
        }
    }
}