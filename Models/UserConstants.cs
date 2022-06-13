using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>()
        {
            new UserModel() { Username = "admin", EmailAddress = "admin@gmail.com", Password = "admin", GivenName = "Admin", Surname = "Adminovsky", Role = "Administrator"},
            new UserModel() { Username = "guest", EmailAddress = "guest@gmail.com", Password = "guest", GivenName = "Guest", Surname = "Guestovsky", Role = "Guest"},
        };
    }
}
