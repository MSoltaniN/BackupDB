namespace BackupDB.API.Dtos
{
    public class DBForBackUpProcessDto
    {
        public string serverName { get; set; }
        public string DBName { get; set; }
        public string DBPath { get; set; }
        public int userId { get; set; }
    }
}