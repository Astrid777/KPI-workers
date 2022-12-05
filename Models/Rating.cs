namespace CustomIdentityApp.Models
{
    public class Rating
    {
        public int Id { get; set; }

        //[Display(Name = "Автор")]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        //[Display(Name = "Дата")]
        public int IndicatorId { get; set; }
        public virtual Indicator Indicator { get; set; }

        public int Value { get; set; }
    }
}
