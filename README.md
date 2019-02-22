
# EmailProviders

An ASP.Net Core 2.2 implementation of the IEmailSender interface of the Microsoft.AspNetCore.Identity.UI package.  This class library includes implementations for Gmail SMTP and SendGrid.

## Email Templates

Store your client specific, HTML formatted, email templates with keyword replacement in your deployed DLL.  You may easily package these as an embedded resource.  The embedded resources do not require any special read/write authorizations, etc. that you would need if you were to put these files outside the document root on the web server.

The email template keyword replacement will be looking for keywords wrapped in curly braces.  For example, the snippet below demonstrates how to add a callbackUrl in the email:

```html
<p>Please confirm your account by clicking&nbsp;<a href="{callbackUrl}">here</a></p>
```

### How To: Create Embedded Resources

In Visual Studio, right-click the html file and choose Properties.  On the Properties window, change Build Action to "Embedded resource."

## Usage

Refer to these examples to successfully use this class library.

## Startup.cs

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IEmailSender, SendGridEmailSender>();
    services.AddSingleton<IEmailTemplateProvider, EmailTemplateProvider>();
    services.Configure<SendGridEmailSenderOptions>(Configuration.GetSection("SendGrid"));
}
```

## appsettings.json

```
{
  "SendGrid": {
    "SendGridUser": "________",
    "SendGridKey": "________",
    "SenderEmail": "noreply@________.com",
    "SenderDisplayName": "________"
  }
}
```

## ForgotPassword.cshtml.cs or Register.cshtml.cs

```
public class ForgotPasswordModel : PageModel
{
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateProvider _emailTemplateProvider;

    public ForgotPasswordModel(IEmailSender emailSender, IEmailTemplateProvider emailTemplateProvider)
    {
        _emailSender = emailSender;
        _emailTemplateProvider = emailTemplateProvider;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var htmlMessage = await _emailTemplateProvider.GetHtmlMessageAsync("foobar.EmailTemplates.ResetPassword.html",
            new Dictionary<string, string>
            {
                { "logoUrl", logoUrl },
                { "callbackUrl", callbackUrl }
            });

        await _emailSender.SendEmailAsync(
            Input.Email,
            "Reset Password",
            htmlMessage);
    }
}
```