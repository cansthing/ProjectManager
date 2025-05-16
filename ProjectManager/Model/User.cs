using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectManager.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname
        {
            get => !string.IsNullOrEmpty(Firstname) || !string.IsNullOrEmpty(Lastname)
                ? $"{Firstname ?? ""}{(string.IsNullOrEmpty(Lastname) ? "" : " ")}{Lastname ?? ""}"
                : "Kein Name vorhanden";
        }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool imageHasBeenSet = false;
        private ImageSource imageSource;
        public ImageSource Image { get => imageSource; set { if (value == imageSource) imageHasBeenSet = true; imageSource = value; } }
        public override string ToString()
        {
            return Fullname;
        }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public User()
        {
            
        }
    }
}
