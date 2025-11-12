using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.SaaS.Accelerator.Services.Configurations
{
    /// <summary>
    /// Configuration for setup of authentication for SMTP using OAuth Client Credentials
    /// </summary>
    public class SmtpOAuthConfiguration
    {
        /// <summary>
        /// OAuth Client ID for email authentication
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// OAuth Client Secret for email authentication
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The authority of the OAuth Token Provider that will be used for authentication
        /// </summary>
        public string TokenAuthority { get; set; }

        /// <summary>
        /// Comma-separated scopes for the OAuth Token Request
        /// </summary>
        public string TokenScopes { get; set; }
    }
}
