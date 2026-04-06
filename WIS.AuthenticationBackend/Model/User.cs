using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WIS.AuthenticationBackend.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
        [JsonIgnore]
        public bool IsLocked { get; set; }
        [JsonIgnore]
        public bool IsEnabled { get; set; }
    }
}
