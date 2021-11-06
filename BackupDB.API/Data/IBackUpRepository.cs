using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackupDB.API.Models;

namespace BackupDB.API.Data
{
    interface IBackUpRepository
    {
        Task<IEnumerable<Database>> GetDatabases();
    }
}
