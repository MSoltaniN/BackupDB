using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    interface IBackUpRepository
    {
        Task<IEnumerable<Database>> GetDatabases();
    }
}
