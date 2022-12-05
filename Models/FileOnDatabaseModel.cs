namespace CustomIdentityApp.Models
{
    public class FileOnDatabaseModel : FileModel
    {
        public byte[] Data { get; set; }

        public string UserName { get; set; }
    }
}
