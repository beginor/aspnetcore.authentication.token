using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Beginor.AspNetCore.Authentication.Token;

public class TokenHandler : AuthenticationHandler<TokenOptions> {

    public TokenHandler(
        IOptionsMonitor<TokenOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder) { }

    protected new TokenEvents? Events {
        get => base.Events as TokenEvents;
        set => base.Events = value;
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
            if (Events != null) {
                await Events.TokenReceived(tokenReceivedContext);
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
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
    private string? GetTokenFromRequest() {
        var token = string.Empty;
        if (Request.Query.TryGetValue(Options.ParamName, out var values)) {
            token = values.ToString();
        }
        if (!string.IsNullOrEmpty(token)) {
            return token;
        }
        string? authorization = Request.Headers.Authorization;
        if (!string.IsNullOrEmpty(authorization)) {
            var scheme = $"{Options.ParamName} ";
            if (authorization.StartsWith(scheme, StringComparison.OrdinalIgnoreCase)) {
                token = authorization.Substring(scheme.Length).Trim();
            }
        }
        return token;
    }
}
