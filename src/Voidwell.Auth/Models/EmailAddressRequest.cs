namespace Voidwell.Auth.Models;

public class EmailAddressRequest
{
    public EmailAddressRequest(string emailAddress)
    {
        EmailAddress = emailAddress;
    }

    public string EmailAddress { get; set; }
}
