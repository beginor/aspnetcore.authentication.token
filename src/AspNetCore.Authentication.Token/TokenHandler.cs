using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Beginor.AspNetCore.Authentication.Token {

    public class TokenHandler : AuthenticationHandler<TokenOptions> {

        public TokenHandler(
            IOptionsMonitor<TokenOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock) { }

        protected new TokenEvents Events {
            get { return base.Events as TokenEvents; }
            set { base.Events = value; }
        }

        protected override Task<object> CreateEventsAsync() {
            return Task.FromResult<object>(new TokenEvents());
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            var token = GetTokenFromRequest();
            if (!string.IsNullOrEmpty(token)) {
                var tokenReceivedContext = new TokenReceivedContext(Context, Scheme, Options) {
                    Token = token
                };
                await Events.TokenReceived(tokenReceivedContext);
                if (tokenReceivedContext.Result != null) {
                    return tokenReceivedContext.Result;
                }
            }
            return AuthenticateResult.NoResult();
        }

        /// <summary>
        /// Get data access token from request, $token in query string
        /// or from Authorization header in `$token data_access_token` format like jwt.
        /// </summary>
        private string GetTokenFromRequest() {
            var token = string.Empty;
            if (Request.Query.TryGetValue(Options.ParamName, out var values)) {
                token = values.FirstOrDefault();
            }
            if (string.IsNullOrEmpty(token)) {
                // todo: refact to typed header in .net 6.x;
                string authorization = Request.Headers[HeaderNames.Authorization];
                if (!string.IsNullOrEmpty(authorization)) {
                    var scheme = $"{Options.ParamName} ";
                    if (authorization.StartsWith(scheme, StringComparison.OrdinalIgnoreCase)) {
                        token = authorization.Substring(scheme.Length).Trim();
                    }
                }
            }
            return token;
        }
    }

}
