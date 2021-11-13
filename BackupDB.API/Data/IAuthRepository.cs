using System.Threading.Tasks;
using BackupDB.API.Models;

namespace BackupDB.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password, string serverPassword);
         Task<User> Login(string username, string password);
         Task<bool> UserExists(string username);
    }
}