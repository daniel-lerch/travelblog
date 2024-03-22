using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelBlog.Models;
using TravelBlog.Services;

namespace TravelBlog.Controllers;

[Route("~/register")]
public class RegistrationController : Controller
{
    private readonly SubscriberService subscriberService;

    public RegistrationController(SubscriberService subscriberService)
    {
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

        // Avoid duplicate registrations with different casing or Gmail domains
        mailAddress = mailAddress.ToLowerInvariant().Replace("@googlemail.com", "@gmail.com");

        if (await subscriberService.Register(mailAddress, givenName, familyName))
            return View("Success", new RegistrationViewModel(mailAddress, givenName, familyName));
        else
            return View("Index", new RegistrationViewModel(mailAddress, givenName, familyName, "Diese E-Mail-Adresse ist bereits registriert."));
    }
}
