using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ScreenCapDictionaryNoteApp.ViewModel.Command.LoginSettingCommand
{
    public class SignupCommand : ICommand
    {
        public LoginSettingsVM LoginSettingVM;

        public SignupCommand(LoginSettingsVM loginSettingVM)
        {
            this.LoginSettingVM = loginSettingVM;
        }



        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }



        public bool CanExecute(object parameter)
        {
            var recaptcha = parameter as String;

            if (recaptcha != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void Execute(object parameter)
        {

            LoginSettingVM.PostRequestToSignup();
        }
    }
}
