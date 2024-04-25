using Microsoft.AspNetCore.Mvc;

namespace FacephiBook.Controllers
{
    public class CatalogoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
