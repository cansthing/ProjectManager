using ProjectManager.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectManager.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
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


        private UserControl userControl;
        public UserControl UserControl
        {
            get { return userControl; }
            set { userControl = value; 
                OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            UserControl = new MainView();
        }

    }
}
