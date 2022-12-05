using System;
using System.ComponentModel.DataAnnotations;

namespace CustomIdentityApp.Models
{
    public class News
    {
        public int Id { get; set; }

        [Display(Name = "Текст новости")]
        public string Text { get; set; }

        //[Display(Name = "Автор")]
        public string UserId { get; set; }
        public User User { get; set; }


        //[Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}
