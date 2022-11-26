using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using TravelBlog.Database;
using TravelBlog.Database.Entities;
using TravelBlog.Models;

namespace TravelBlog.Controllers;

[Route("~/{action=Index}")]
public class HomeController : Controller
{
	private readonly DatabaseContext database;

	public HomeController(DatabaseContext database)
	{
		this.database = database;
	}

	public async Task<IActionResult> Index()
	{
		Page? homePage = await database.Pages.SingleOrDefaultAsync(p => p.Name == "Homepage");
		return View(new HomeViewModel(homePage?.Content));
	}

	[HttpGet("~/home/edit")]
	[Authorize(Roles = Constants.AdminRole)]
	public async Task<IActionResult> Edit()
	{
		Page? homePage = await database.Pages.SingleOrDefaultAsync(p => p.Name == "Homepage");
		return View(new HomeEditViewModel(homePage?.Content ?? string.Empty));
	}

	[HttpPost("~/home/edit")]
	[Authorize(Roles = Constants.AdminRole)]
	public async Task<IActionResult> Update(string content)
	{
		Page? homePage = await database.Pages.SingleOrDefaultAsync(p => p.Name == "Homepage");
		if (homePage == null)
		{
			database.Pages.Add(homePage = new Page("Homepage", content));
		}
		else
		{
			homePage.Content = content;
		}
		await database.SaveChangesAsync();

		return Redirect("~/");
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
