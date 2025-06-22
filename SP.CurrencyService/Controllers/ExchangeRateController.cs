using Microsoft.AspNetCore.Mvc;

namespace SP.CurrencyService.Controllers;

public class ExchangeRateController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}