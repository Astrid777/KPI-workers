using Microsoft.AspNetCore.Identity;
using System;

namespace CustomIdentityApp.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        public string Position { get; set; }

        public bool IsFamiliarized { get; set; }

        public DateTime FamiliarizationTime { get; set; }
    }
}