using Microsoft.AspNetCore.Mvc;

public class PacientesController : Controller
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