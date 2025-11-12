using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Marketplace.SaaS.Accelerator.Services.Contracts
{
    /// <summary>
    /// Generic interface for getting an access token to pass to the SMTP service
    /// </summary>
    public interface ISmtpOAuthTokenProvider
    {
        /// <summary>
        /// Implement the logic to make the call to retrieve the access token
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationResult> GetAccessToken(CancellationToken cancellationToken = default);
    }
}