using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ProjectManager.Model;
using ProjectManager.DataProvider;
using System.Windows.Input;
using ProjectManager.Commands;
using ProjectManager.View;

namespace ProjectManager.ViewModel
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public User CurrentUser
        {
            get => ObjectRepository.DataProvider.CurrentUser; 
        }

        public ICommand EditMyAccountCommand => new MyICommand(EditMyAccount);

        private async void EditMyAccount(object obj)
        {
            var dialog = new CreateUser(CurrentUser)
            {
                Title = "Account bearbeiten"
            };
            await dialog.ShowAsync();
            try
            {
                await ObjectRepository.DataProvider.UpdateUser(CurrentUser);
            }
            catch { }
        }

        public AccountViewModel()
        {
            
        }
    }
}
