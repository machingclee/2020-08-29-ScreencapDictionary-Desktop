using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenCapDictionaryNoteApp.Model
{
    public class AuthLoginRequest
    {
        public string username { get; set; }

        public string password { get; set; }

        public AuthLoginRequest(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
