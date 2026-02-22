using Microsoft.AspNetCore.Mvc;

namespace MVC.CitasMedicas.Controllers
{
    public class CitasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}