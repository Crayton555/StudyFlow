using Microsoft.AspNetCore.Identity;

namespace StudyFlow.Models.Identity
{
    public class StudyFlowUser : IdentityUser
    {
        public ICollection<Domain.Task>? Tasks { get; set; }
    }
}
