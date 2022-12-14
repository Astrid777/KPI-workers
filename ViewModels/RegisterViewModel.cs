using System.ComponentModel.DataAnnotations;

namespace CustomIdentityApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "ФИО")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Отдел")]
        public int DepartmentId { get; set; }

        [Required]
        [Display(Name = "Должность")]
        public string Position { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Поле {0} должно иметь минимум {2} и максимум {1} символов.", MinimumLength = 6)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]

        public string PasswordConfirm { get; set; }
    }
}