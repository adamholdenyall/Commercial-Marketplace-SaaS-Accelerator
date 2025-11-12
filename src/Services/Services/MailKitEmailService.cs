using System;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Marketplace.SaaS.Accelerator.Services.Services
{
    /// <summary>
    /// Service to send emails using modern authentication. Leverages the MailKit library.
    /// </summary>
    /// <seealso cref="IEmailService" />
    public class MailKitEmailService : IEmailService
    {
        /// <summary>
        /// The application configuration repository.
        /// </summary>
        private readonly IApplicationConfigRepository applicationConfigRepository;

        /// <summary>
        /// The application log repository.
        /// </summary>
        private readonly IApplicationLogRepository applicationLogRepository;

        /// <summary>
        /// The provider for OAuth tokens used to authenticate against the email server
        /// </summary>
        private readonly ISmtpOAuthTokenProvider smtpOAuthTokenProvider;

        /// <summary>
        /// The application log service.
        /// </summary>
        private readonly ApplicationLogService applicationLogService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailService"/> class.
        /// </summary>
        /// <param name="applicationConfigRepository">The application configuration repository.</param>
        public MailKitEmailService(IApplicationConfigRepository applicationConfigRepository,
                                IApplicationLogRepository applicationLogRepository,
                                ISmtpOAuthTokenProvider smtpOAuthTokenProvider)
        {
            this.applicationConfigRepository = applicationConfigRepository;
            this.applicationLogRepository = applicationLogRepository;
            this.smtpOAuthTokenProvider = smtpOAuthTokenProvider;
            this.applicationLogService = new ApplicationLogService(this.applicationLogRepository);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="emailContent">Content of the email.</param>
        public void SendEmail(EmailContentModel emailContent)
        {
            if (!string.IsNullOrEmpty(emailContent.ToEmails) || !string.IsNullOrEmpty(emailContent.BCCEmails))
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(emailContent.FromEmail, emailContent.FromEmail));
                    message.Subject = emailContent.Subject;

                    var bodyBuilder = new BodyBuilder { HtmlBody = emailContent.Body };
                    message.Body = bodyBuilder.ToMessageBody();

                    foreach (var to in emailContent.ToEmails.Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(MailboxAddress.Parse(to));
                    }

                    foreach (var bcc in emailContent.BCCEmails?.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>())
                    {
                        message.Bcc.Add(MailboxAddress.Parse(bcc));
                    }

                    using (var client = new SmtpClient())
                    {
                        client.Connect(emailContent.SMTPHost, emailContent.Port, emailContent.SSL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

                        // Use OAuth2 token instead of basic username/password
                        var authResult = smtpOAuthTokenProvider.GetAccessToken().GetAwaiter().GetResult();
                        var oauth2 = new SaslMechanismOAuth2(emailContent.UserName, authResult.AccessToken);
                        client.Authenticate(oauth2);

                        client.Send(message);
                        client.Disconnect(true);
                    }

                    this.applicationLogService.AddApplicationLog($"{emailContent?.Subject}: Email sent successfully!").ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    applicationLogService.AddApplicationLog($"{emailContent?.Subject}: Exception {ex.Message}.").ConfigureAwait(false);
                }
            }
            else
            {
                applicationLogService.AddApplicationLog($"{emailContent?.Subject}: Email not sent because the To email address is empty.").ConfigureAwait(false);
            }
        }
    }
}
