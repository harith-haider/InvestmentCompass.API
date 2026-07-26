using System.ComponentModel.DataAnnotations.Schema;

namespace InvestmentCompass.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal FundingGoal { get; set; }
        
        public string Description { get; set; } = string.Empty;
        
        // حالة المشروع: "Pending", "Approved", "Rejected"
        public string Status { get; set; } = "Pending"; 
        
        // ربط المشروع بصاحبه (علاقة Foreign Key)
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}