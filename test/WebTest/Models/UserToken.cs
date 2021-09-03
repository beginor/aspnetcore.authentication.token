using System.Collections.Generic;

namespace WebTest.Models {

    public class UserToken {
        public string Id { get; set; }
        public string Username { get; set; }
        public IList<string> Roles { get; set; }
    }

}
