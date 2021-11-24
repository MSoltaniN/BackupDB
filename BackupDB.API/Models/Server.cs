namespace BackupDB.API.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
        public User user { get; set; }
        public int? UserId { get; set; }
    }
}