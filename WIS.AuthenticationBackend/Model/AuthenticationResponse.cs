using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.AuthenticationBackend.Model
{
    public class AuthenticationResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public AuthenticationResponse(User user)
        {
            this.UserId = user.UserId;
            this.Username = user.Username;
            this.Name = user.Name;
            this.Email = user.Email;
        }
    }
}
