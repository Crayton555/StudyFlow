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
        public DateTime CreatedAt { get; set; }
        [Required]
        [AfterDate("CreatedAt", ErrorMessage = "Due Date must be after Created At.")]
        public DateTime DueDate { get; set; }
        public StudyFlowUser? User { get; set; }
    }

    public class AfterDateAttribute : ValidationAttribute
    {
        private readonly string _compareToPropertyName;

        public AfterDateAttribute(string compareToPropertyName)
        {
            _compareToPropertyName = compareToPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_compareToPropertyName);

            if (property == null)
            {
                return new ValidationResult($"Property {_compareToPropertyName} not found.");
            }

            var compareToValue = property.GetValue(validationContext.ObjectInstance);

            if (compareToValue == null || value == null || (DateTime)value > (DateTime)compareToValue)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
