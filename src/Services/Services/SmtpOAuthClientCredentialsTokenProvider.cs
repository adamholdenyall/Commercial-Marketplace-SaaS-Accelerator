using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.Identity.Client;

namespace Marketplace.SaaS.Accelerator.Services.Services
{
    /// <summary>
    /// Pulls configuration of the client ID and secret to generate an access token to send for authentication to the SMTP service
    /// </summary>
    public class SmtpOAuthClientCredentialsTokenProvider : ISmtpOAuthTokenProvider
    {
        /// <summary>
        /// Holds a reference to the configuration information
        /// </summary>
        private readonly SmtpOAuthConfiguration smtpOAuthConfiguration;

        /// <summary>
        /// Constructor that saves the configuration for use when getting the access token
        /// </summary>
        /// <param name="smtpOAuthConfiguration"></param>
        public SmtpOAuthClientCredentialsTokenProvider(SmtpOAuthConfiguration smtpOAuthConfiguration)
        {
            this.smtpOAuthConfiguration = smtpOAuthConfiguration;
        }

        /// <summary>
        /// Leverages the Microsoft.Identity.Client to get the access token
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> GetAccessToken(CancellationToken cancellationToken = default)
        {
            var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(smtpOAuthConfiguration.ClientId)
                .WithAuthority(smtpOAuthConfiguration.TokenAuthority)
                .WithClientSecret(smtpOAuthConfiguration.ClientSecret)
                .Build();

            string[] scopes = smtpOAuthConfiguration.TokenScopes.Split(',');

            return await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken);
        }
    }
}
