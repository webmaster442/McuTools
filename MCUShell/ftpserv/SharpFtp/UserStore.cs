using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace SharpFtpServer
{
    // TODO: Implement a real user store.
    [Obsolete]
    public static class UserStore
    {
        private static List<User> _users;

        static UserStore()
        {
            _users = new List<User>();

            XmlSerializer serializer = new XmlSerializer(_users.GetType(), new XmlRootAttribute("Users"));

            if (File.Exists("users.xml"))
            {
                _users = serializer.Deserialize(new StreamReader("users.xml")) as List<User>;
            }
            else
            {
                _users.Add(new User {
                    Username = "rick",
                    Password = "test",
                    HomeDir = "C:\\Utils"
                });

                using (StreamWriter w = new StreamWriter("users.xml"))
                {
                    serializer.Serialize(w, _users);
                }
            }
        }

        public static User Validate(string username, string password)
        {
            User user = (from u in _users where u.Username == username && u.Password == password select u).SingleOrDefault();

            return user;
        }
    }
}
