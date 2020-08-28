using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ScreenCapDictionaryNoteApp.ViewModel.Command.LoginSettingCommand
{
    public class OpenLoginSettingCommand : ICommand
    {
        private MainVM mainVM;

        public event EventHandler CanExecuteChanged;


        public OpenLoginSettingCommand(MainVM mainVM)
        {
            this.mainVM = mainVM;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mainVM.OpenLoginSetting();
        }
    }
}
