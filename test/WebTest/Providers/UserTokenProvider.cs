using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using WebTest.Models;

namespace WebTest.Providers; 

public class UserTokenProvider {

    private IList<UserToken> tokens;

    public UserTokenProvider(IOptions<UserTokenOptions> options) {
        tokens = options.Value.Tokens;
    }

    public UserToken GetById(string id) {
        return this.tokens.FirstOrDefault(tk => tk.Id == id);
    }

}

public class UserTokenOptions {

    public IList<UserToken> Tokens { get; set; } = new List<UserToken>();

}