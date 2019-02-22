using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AWJ.EmailProviders
{
    /// <summary>
    /// Gmail SMTP server
    /// </summary>
    /// <remarks>
    /// Gmail SMTP server allows for 2,000 Messages per day.
    /// 
    /// enabling less secure apps
    /// https://www.google.com/settings/security/lesssecureapps
    /// 
    /// See also:
    /// https://hassantariqblog.wordpress.com/2017/03/20/asp-net-core-sending-email-with-gmail-account-using-asp-net-core-services/
    /// </remarks>
    public class GmailEmailSender : IEmailSender
    {
        public GmailEmailSenderOptions Options { get; } //set only via Secret Manager
        private readonly ILogger _logger;

        public GmailEmailSender(IOptions<GmailEmailSenderOptions> optionsAccessor, ILogger<GmailEmailSender> logger)
        {
            Options = optionsAccessor.Value;
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Entering SendEmailAsync (email={email}, subject={subject}, htmlMessage={htmlMessage})");
            }

            Execute(subject, htmlMessage, email).Wait();
            return Task.FromResult(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject">Subject of the email</param>
        /// <param name="htmlMessage">Body of the email</param>
        /// <param name="email">To email address </param>
        /// <returns></returns>
        public async Task Execute(string subject, string htmlMessage, string email)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Entering Execute (subject={subject}, htmlMessage={htmlMessage}, email={email})");
            }

            if (subject == null) throw new ArgumentNullException(nameof(subject));
            if (subject == null) throw new ArgumentNullException(nameof(htmlMessage));
            if (subject == null) throw new ArgumentNullException(nameof(email));

            try
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(Options.SenderEmail, Options.SenderDisplayName),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = htmlMessage
                };
                msg.To.Add(email);

                var client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Options.GmailUserName, Options.GmailPassword)
                };

                await client.SendMailAsync(msg);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Exception thrown in Execute (subject={subject}, htmlMessage={htmlMessage}, email={email})");
                throw ex;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// In the Azure key vault, create a secret named "GmailEmailSenderOptions".
    /// The value is this class serialized to a JSON string. An example is shown below.
    /// Set the content type as "application/json"
    /// 
    /// JSON for key vault:
    /// { "GmailUserName": "", "GmailPassword": "", "SenderEmail": "", "SenderDisplayName": "" }
    /// </remarks>
    [DebuggerDisplay("SenderEmail={SenderEmail}, GmailUserName={GmailUserName}")]
    public class GmailEmailSenderOptions
    {
        public string GmailUserName { get; set; }
        public string GmailPassword { get; set; }

        /// <summary>
        /// Gets or sets the email address of the sender or recipient.
        /// </summary>
        public string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets the name of the sender or recipient.
        /// </summary>
        public string SenderDisplayName { get; set; }
    }
}
