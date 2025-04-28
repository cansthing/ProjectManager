using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        private async void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            LoginViewModel.Password = password.Password;
            if (await ObjectRepository.DataProvider.Login(new Model.User(LoginViewModel.Username, LoginViewModel.Password)))
            {
                this.Close();
            }
            else
            {
                MessageText.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                MessageText.Visibility = Visibility.Collapsed;
            }
        }
    }
}
