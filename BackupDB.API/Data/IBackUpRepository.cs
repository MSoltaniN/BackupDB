using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackupDB.API.Models;

namespace BackupDB.API.Data
{
    public interface IBackUpRepository
    {
        Task<IEnumerable<Database>> GetDatabases();
        Task<Server> Add(Server server);
         Task<Server> GetServerInfo(int userId);
    }
}
