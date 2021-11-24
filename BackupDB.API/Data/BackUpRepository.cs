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

        public async Task<Server> Add(Server server)
        {
            await _context.Servers.AddAsync(server);
            await _context.SaveChangesAsync();
            return server;
        }

        public Task<Server> GetServerInfo(int userId)
        {
            return _context.Servers.FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}
