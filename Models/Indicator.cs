using System.ComponentModel.DataAnnotations;

namespace CustomIdentityApp.Models
{
    public class Indicator
    {
        public int Id { get; set; }

        [Display(Name = "Показатель")]
        public string Name { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
