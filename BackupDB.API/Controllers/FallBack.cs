using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace BackupDB.API.Controllers
{
    public class FallBack : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),
            "wwwroot","index.html"),"text/HTML");
        }
        
    }
}