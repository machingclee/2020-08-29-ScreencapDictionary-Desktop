using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Newtonsoft.Json;
using ScreenCapDictionaryNoteApp.Model;
using ScreenCapDictionaryNoteApp.ViewModel;
using ScreenCapDictionaryNoteApp.ViewModel.Command.LoginSettingCommand;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenCapDictionaryNoteApp.View
{

    public partial class LoginSettingsWindow : Window
    {


        private LoginSettingsVM loginSettingVM { get; set; }


        public LoginSettingsWindow()
        {
            InitializeComponent();



            loginSettingVM = Resources["loginVM"] as LoginSettingsVM;


            webView.NavigateToLocalStreamUri(new Uri("recaptcha.html", UriKind.Relative), new CustomResolver());

            //Webview Events
            webView.ScriptNotify += BindRecaptchaToVM;

            //Close Login settings when sign up post request is done:
            LoginSettingsVM.CloseLoginSettingForm += LoginSettingsVM_CloseLoginSettingForm;

            //Show sign up fail message box when sign up post request fails and close the login form:
            LoginSettingsVM.ShowSignupFaileMessageBox += LoginSettingsVM_ShowSignupFaileMessageBox;


        }



        private void LoginSettingsVM_CloseLoginSettingForm(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoginSettingsVM_ShowSignupFaileMessageBox(object sender, EventArgs e)
        {
            MessageBox.Show("Sign up request fails, please try again.", "Fail Message");
            this.Close();
        }





        private void BindRecaptchaToVM(object sender, WebViewControlScriptNotifyEventArgs e)
        {
            loginSettingVM.Recaptcha = e.Value;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            loginSettingVM.UpdateApplicationSetting();
            this.Close();
        }


        private void LoginButtonClickHandler(object sender, RoutedEventArgs e)
        {
            loginSettingVM.fetchJwtTokenFromServer();
        }

        class CustomResolver : IUriToStreamResolver
        {
            public Stream UriToStream(Uri uri)
            {
                Stream fileStream = File.OpenRead(System.IO.Path.Combine(Environment.CurrentDirectory, uri.LocalPath.TrimStart('/')));
                return fileStream;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
