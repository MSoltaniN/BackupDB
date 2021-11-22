using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackupDB.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BackupDB.API.Data
{
    public class BackUpRepository : IBackUpRepository
    {
        private readonly DataContext _context;
        public Task<IEnumerable<Database>> GetDatabases()
        {
            throw new NotImplementedException();
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public Task<Server> GetServerInfo(int userId)
        {
           return  _context.Servers.FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}
