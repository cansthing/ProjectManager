using Microsoft.Win32;
using ModernWpf.Controls;
using ProjectManager.Model;
using ProjectManager.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager.View
{
    /// <summary>
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : ContentDialog
    {
        public CreateUser(User user)
        {
            InitializeComponent();
            VM.NewUser = user ?? new User();

        }

        private void ContentDialog_PrimaryButtonClick(ModernWpf.Controls.ContentDialog sender, ModernWpf.Controls.ContentDialogButtonClickEventArgs args)
        {
            VM.NewUser.Password = password.Password;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Profilbild auswählen"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var filePath = dialog.FileName;

                    if (!File.Exists(filePath)) 
                    {
                        MessageBox.Show("Die Datei existiert nicht.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(filePath);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();

                    VM.NewUser.Image = image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden des Bildes: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
