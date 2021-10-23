using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public class BackUpRepository : IBackUpRepository
    {
        public Task<IEnumerable<Database>> GetDatabases()
        {
            throw new NotImplementedException();
        }
    }
}
