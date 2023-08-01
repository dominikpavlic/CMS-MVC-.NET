using Microsoft.AspNetCore.Mvc;

namespace CMSProductSystem.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
