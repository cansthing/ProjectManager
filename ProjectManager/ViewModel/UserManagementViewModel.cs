using ProjectManager.Commands;
using ProjectManager.DataProvider;
using ProjectManager.Model;
using ProjectManager.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectManager.ViewModel
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool CanCurrentUser
        {
            get => ObjectRepository.DataProvider.CurrentUser.IsAdmin;
        }
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        private User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set { selectedUser = value;
                OnPropertyChanged();
            }
        }

        private User newUser;
        public User NewUser
        {
            get { return newUser; }
            set { newUser = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateUserCommand => new MyICommand(CreateUser);
        public ICommand OpenEditUserCommand => new MyICommand(OpenEditUser);
        public ICommand SaveUserCommand => new MyICommand(SaveUser);
        public ICommand DeleteUserCommand => new MyICommand(DeleteUser);

        public UserManagementViewModel()
        {
            Users = new ObservableCollection<User>();
            NewUser = new User();
            LoadUsers();
        }
        public async void LoadUsers()
        {
            Users = await ObjectRepository.DataProvider.GetUsers();
        }

        private async void CreateUser(object obj)
        {
            var dialog = new CreateUser(new User());
            await dialog.ShowAsync();
            LoadUsers();
        }
        private async void OpenEditUser(object obj)
        {
            NewUser = SelectedUser;
            var dialog = new CreateUser(NewUser);
            dialog.Title = "Benutzer bearbeiten";
            await dialog.ShowAsync();
            LoadUsers();
        }
        private async void SaveUser(object obj)
        {
            if (NewUser == new User()) return;

            if (NewUser.Id == 0)
                await ObjectRepository.DataProvider.CreateUser(NewUser);
            else
                await ObjectRepository.DataProvider.UpdateUser(NewUser);
            LoadUsers();
        }
        private async void DeleteUser(object obj)
        {
            var result = MessageBox.Show("Möchten Sie den Benutzer wirklich löschen?", "Benutzer löschen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;
            if (SelectedUser == null) return;
            if (SelectedUser.Id == ObjectRepository.DataProvider.CurrentUser.Id)
            {
                MessageBox.Show("Sie können sich nicht selbst löschen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SelectedUser.IsAdmin)
            {
                MessageBox.Show("Sie können den Administrator nicht löschen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            await ObjectRepository.DataProvider.DeleteUser(SelectedUser);
            LoadUsers();
        }


    }
}
