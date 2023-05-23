using StudyFlow.Models.Domain.Enumeration;
using StudyFlow.Models.Identity;
using System.ComponentModel.DataAnnotations;


namespace StudyFlow.Models
{
    public class Calendar
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
