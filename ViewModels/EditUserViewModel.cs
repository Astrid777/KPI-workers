namespace CustomIdentityApp.ViewModels
{
    //public class CreateUserViewModel
    //{
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //    public string Name { get; set; }
    //}

    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
