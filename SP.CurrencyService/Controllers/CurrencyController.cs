using Microsoft.AspNetCore.Mvc;

namespace SP.CurrencyService.Controllers;

public class CurrencyController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}