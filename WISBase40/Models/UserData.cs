using System.Collections.Generic;

namespace WIS.WebApplication.Models
{
    public class UserData
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Language { get; set; }
        public bool IsModeTablet { get; set; }
        public string Predio { get; set; }
        public List<string> Predios { get; set; }

        public UserData()
        {
            Predios = new List<string>();
        }
    }
}
