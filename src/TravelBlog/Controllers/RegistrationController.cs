using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelBlog.Database;
using TravelBlog.Models;
using TravelBlog.Services;

namespace TravelBlog.Controllers;

[Route("~/register")]
public class RegistrationController : Controller
{
    private readonly DatabaseContext context;
    private readonly SubscriberService subscriberService;

    public RegistrationController(DatabaseContext context, SubscriberService subscriberService)
    {
        this.context = context;
        this.subscriberService = subscriberService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return View("Index", new RegistrationViewModel("", "", ""));
    }

    [HttpPost]
    public async Task<IActionResult> Post([EmailAddress] string mailAddress, [Required] string givenName, [Required] string familyName)
    {
        if (!ModelState.IsValid)
            return View("Index", new RegistrationViewModel(mailAddress, givenName, familyName, "Deine Angaben sind unvollständig oder ungültig!"));

        if (await subscriberService.Register(mailAddress, givenName, familyName))
            return View("Success", new RegistrationViewModel(mailAddress, givenName, familyName));
        else
            return View("Index", new RegistrationViewModel(mailAddress, givenName, familyName, "Diese E-Mail-Adresse ist bereits registriert."));
    }
}
