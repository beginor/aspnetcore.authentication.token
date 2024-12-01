using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Beginor.AspNetCore.Authentication.Token;

public class TokenReceivedContext : ResultContext<TokenOptions> {

    public TokenReceivedContext(
        HttpContext context,
        AuthenticationScheme scheme,
        TokenOptions options
    ) : base(context, scheme, options) { }

    public string? Token { get; set; }

}
