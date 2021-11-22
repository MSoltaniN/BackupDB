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
        void Add<T>(T entity) where T : class;
         Task<Server> GetServerInfo(int userId);
    }
}
