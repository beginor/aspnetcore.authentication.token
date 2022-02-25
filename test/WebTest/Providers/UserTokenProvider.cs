using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using WebTest.Models;

namespace WebTest.Providers;

public class UserTokenProvider {

    private readonly IList<UserToken> tokens;

    public UserTokenProvider(IOptions<UserTokenOptions> options) {
        tokens = options.Value.Tokens;
    }

    public UserToken? GetById(string id) {
        if (id == null) {
            throw new ArgumentNullException(nameof(id));
        }
        return tokens.FirstOrDefault(tk => tk.Id == id);
    }

}

public class UserTokenOptions {

    public IList<UserToken> Tokens { get; set; } = new List<UserToken>();

}
