using StudyFlow.Models.Domain.Enumeration;
using StudyFlow.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Models.Domain
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public Priority? Priority { get; set; }
        [Required]
        public Status? Status { get; set; }
        [Required]
        /*[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]*/
        public DateTime CreatedAt { get; set; }
        [Required]
        /*[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]*/
        public DateTime DueDate { get; set; }
        public StudyFlowUser? User { get; set; }
    }
}
