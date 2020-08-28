using Newtonsoft.Json;
using ScreenCapDictionaryNoteApp.Model;
using ScreenCapDictionaryNoteApp.ViewModel.Command.LoginSettingCommand;
using ScreenCapDictionaryNoteApp.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

namespace ScreenCapDictionaryNoteApp.ViewModel
{
    public class LoginSettingsVM : INotifyPropertyChanged
    {

        private string _Recaptcha;

        public string Recaptcha
        {
            get { return _Recaptcha; }
            set
            {
                _Recaptcha = value;
                onPropertyChanged("Recaptcha");
                CommandManager.InvalidateRequerySuggested();

            }
        }

        public SignupCommand SignupCommand { get; set; }

        private string _Username;

        public string Username
        {
            get { return _Username; }
            set
            { _Username = value; onPropertyChanged("UserName"); }
        }


        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value; onPropertyChanged("Password"); }
        }


        private string _JwtToken;

        public string JwtToken
        {
            get { return _JwtToken; }
            set
            {
                _JwtToken = value;
                applicationSetting.jwtToken = value;
                onPropertyChanged("JwtToken");
            }
        }


        private ApplicationConfig applicationSetting { get; set; }


        private ApplicationConfig _ApplicationSetting;

        public ApplicationConfig ApplicationSetting
        {
            get { return _ApplicationSetting; }
            set { _ApplicationSetting = value; }
        }





        public ICommand LoginCommand { get; set; }



        public LoginSettingsVM()
        {
            getApplicationConfig();
            SignupCommand = new SignupCommand(this);

            JwtToken = applicationSetting.jwtToken;
        }



        public async void fetchJwtTokenFromServer()
        {

            using (var client = new System.Net.Http.HttpClient())
            {
                try
                {
                    String loginUrl = Config.SERVICE_HOST + "/auth/login";

                    var jsonStringForUpload = JsonConvert.SerializeObject(
                            new AuthLoginRequest(Username, Password)
                        );


                    var uploadContent = new StringContent(jsonStringForUpload, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(loginUrl, uploadContent);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var jwtJsonResponseString = await response.Content.ReadAsStringAsync();
                        var jwtTokenResponse = JsonConvert.DeserializeObject<JwtTokenResponse>(jwtJsonResponseString);


                        JwtToken = jwtTokenResponse.jwtToken;
                    }
                    else
                    {
                        JwtToken = "Login fails";
                    }
                }
                catch (Exception e)
                {
                    JwtToken = "Login fails";
                }
            }



        }



        public void updateApplicationSetting()
        {
            DatabaseHelper.Update(applicationSetting);
        }



        private void getApplicationConfig()
        {
            List<ApplicationConfig> config = DatabaseHelper.Read<ApplicationConfig>();


            if (config.Count == 0)
            {

                applicationSetting = new ApplicationConfig();
                DatabaseHelper.Insert(applicationSetting);
            }
            else
            {
                applicationSetting = config[0];

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




        public async void PostRequestToSignup()
        {
            String postRequestEndPoint = Config.SERVICE_HOST + "/auth/signup";


            using (var client = new HttpClient())
            {
                String jsonStringForUpdate = JsonConvert.SerializeObject(new RecaptchaPostRequest()
                {
                    username = Username,
                    password = Password,
                    recaptchaResponse = Recaptcha
                });

                var postRequestContent = new StringContent(jsonStringForUpdate, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(postRequestEndPoint, postRequestContent);
                    var jwtJsonResponseString = await response.Content.ReadAsStringAsync();
                    var jwtTokenResponse = JsonConvert.DeserializeObject<JwtTokenResponse>(jwtJsonResponseString);


                    JwtToken = jwtTokenResponse.jwtToken;

                }
                catch (Exception err)
                {
                    JwtToken = "Signup fails";
                    JwtToken += postRequestEndPoint;
                }
            }
        }


    }

}
