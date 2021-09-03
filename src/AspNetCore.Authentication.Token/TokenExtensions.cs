using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Beginor.AspNetCore.Authentication.Token {

    public static class TokenExtensions {

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder)
            => builder.AddToken(TokenOptions.DefaultSchemaName, _ => { });

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, Action<TokenOptions> configureOptions)
            => builder.AddToken(TokenOptions.DefaultSchemaName, configureOptions);

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, string authenticationScheme, Action<TokenOptions> configureOptions)
            => builder.AddToken(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddToken(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TokenOptions> configureOptions) {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<TokenOptions>, TokenPostConfigureOptions>());
            return builder.AddScheme<TokenOptions, TokenHandler>(authenticationScheme, displayName, configureOptions);
        }

    }

}
