using Microsoft.AspNetCore.Authentication;

namespace Beginor.AspNetCore.Authentication.Token {

    public class TokenOptions : AuthenticationSchemeOptions {

        public static readonly string DefaultSchemaName = "data-access-token";
        public string ParamName { get; set; } = "$token";

        public new TokenEvents Events {
            get { return base.Events as TokenEvents; }
            set { base.Events = value; }
        }

    }

}
