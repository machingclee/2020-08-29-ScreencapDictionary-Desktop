using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenCapDictionaryNoteApp.Model
{
    public class ApplicationConfig
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string jwtToken { get; set; }
                public string username { get; set; }
        public string password { get; set; }



        public ApplicationConfig()
        {
            jwtToken = "";
            username = "";
            password = "";
        }

    }
}
