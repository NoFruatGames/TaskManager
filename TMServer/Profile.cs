using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMServer
{
    internal class Profile
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Profile()
        {

        }
        public Profile(Profile profile)
        {
            this.Password = profile.Password;
            this.Email = profile.Email;
            this.Username = profile.Username;
        }
        public override string ToString()
        {
            return $"{Username}\t{Password}\t{Email}";
        }
    }
}
