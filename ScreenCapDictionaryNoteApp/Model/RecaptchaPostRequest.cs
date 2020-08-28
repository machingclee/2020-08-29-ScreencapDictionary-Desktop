using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenCapDictionaryNoteApp.Model
{
    public class RecaptchaPostRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string recaptchaResponse { get; set; }
    }
}
