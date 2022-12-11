using Microsoft.Extensions.Options;

namespace Beginor.AspNetCore.Authentication.Token;

public class TokenPostConfigureOptions : IPostConfigureOptions<TokenOptions> {

    public void PostConfigure(string? name, TokenOptions options) {
        // throw new System.NotImplementedException();
    }

}
