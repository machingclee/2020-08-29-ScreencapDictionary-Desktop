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
            {
                _Username = value;
                onPropertyChanged("UserName");
            }
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
                ApplicationConfig.jwtToken = value;
                onPropertyChanged("JwtToken");
            }
        }


        private ApplicationConfig _ApplicationConfig;

        public ApplicationConfig ApplicationConfig
        {
            get { return _ApplicationConfig; }
            set { _ApplicationConfig = value; }
        }





        public ICommand LoginCommand { get; set; }



        public LoginSettingsVM()
        {
            ApplicationConfig = getCurrentApplicationConfig();

            SignupCommand = new SignupCommand(this);

            JwtToken = ApplicationConfig.jwtToken;
            Username = ApplicationConfig.username;
            Password = ApplicationConfig.password;
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
            ApplicationConfig.username = Username;
            ApplicationConfig.password = Password;
            DatabaseHelper.Update(ApplicationConfig);
        }



        private ApplicationConfig getCurrentApplicationConfig()
        {
            List<ApplicationConfig> config = DatabaseHelper.Read<ApplicationConfig>();
            var currentConfig = new ApplicationConfig();

            if (config.Count == 0)
            {
                DatabaseHelper.Insert(currentConfig);
            }
            else
            {
                currentConfig = config[0];
            }

            return currentConfig;
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
