using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomIdentityApp.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Display(Name = "Название отдела")]
        public string Name { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Indicator> Indicators { get; set; }

        public Department()
        {
            Users = new List<User>();
            Indicators = new List<Indicator>();
        }
    }
}
