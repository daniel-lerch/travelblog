using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TravelBlog.Models;

namespace TravelBlog.Controllers;

[Route("~/{action=Index}")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("{code}")]
    public IActionResult Status(int code)
    {
        return View(new StatusViewModel(code, ReasonPhrases.GetReasonPhrase(code)));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
