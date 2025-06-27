using ProjectManager.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectManager
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
            keyf4.Command = new MyICommand(Exit);
        }

        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            await TryToLogin();
        }

        private async void btn_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await TryToLogin();
            }
        }
        private async Task TryToLogin()
        {
            LoginViewModel.IsBusy = true;
            LoginViewModel.Password = password.Password;

            bool loginSuccess = await Task.Run(async () =>
                await ObjectRepository.DataProvider.Login(new Model.User(LoginViewModel.Username, LoginViewModel.Password)));

            if (loginSuccess)
            {
                Dispatcher.Invoke(() => this.Close());
            }
            else
            {
                MessageText.Visibility = Visibility.Visible;
                await Task.Delay(3000);
                MessageText.Visibility = Visibility.Collapsed;
            }

            LoginViewModel.IsBusy = false;
        }
        private void Exit(object obj)
        {
            this.Close();
            Application.Current.Shutdown();
        }
    }
}
