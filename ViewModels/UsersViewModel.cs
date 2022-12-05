using System.ComponentModel.DataAnnotations;

namespace CustomIdentityApp.ViewModels
{
    public class UsersViewModel
    {
        public string Id;

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "ФИО")]
        public string Name { get; set; }

        [Display(Name = "Отдел")]
        public string? Department { get; set; }

        [Display(Name = "Должность")]
        public string Position { get; set; }

        [Display(Name = "Роль")]
        public string Role { get; set; }

        [Display(Name = "Баллы")]
        public int SummPoints { get; set; }

        [Display(Name = "Ознакомлен")]
        public bool IsFamiliarized { get; set; }
    }
}
