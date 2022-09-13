namespace TravelBlog.Models;

public class RegistrationViewModel
{
    public RegistrationViewModel(string mailAdress, string givenName, string familyName)
        : this(mailAdress, givenName, familyName, null) { }

    public RegistrationViewModel(string mailAddress, string givenName, string familyName, string? displayMessage)
    {
        MailAddress = mailAddress;
        GivenName = givenName;
        FamilyName = familyName;
        DisplayMessage = displayMessage;
    }

    public string MailAddress { get; }
    public string GivenName { get; }
    public string FamilyName { get; }
    public string? DisplayMessage { get; }
}
