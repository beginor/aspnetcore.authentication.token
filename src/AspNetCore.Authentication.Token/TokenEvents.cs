
using System;
using System.Threading.Tasks;

namespace Beginor.AspNetCore.Authentication.Token {

    public class TokenEvents {

        public Func<TokenReceivedContext, Task> OnTokenReceived { get; set; } = context => Task.CompletedTask;

        public virtual Task TokenReceived(TokenReceivedContext context) => OnTokenReceived(context);

    }

}
