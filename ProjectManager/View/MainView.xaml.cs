using ModernWpf.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectManager.View
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
        private Stack<UserControl> navigationStack = new Stack<UserControl>();
        public MainView()
		{
			InitializeComponent();
		}
		UserControl StartView = new StartView();
		UserControl ProjectsView = new ProjectsView();
		UserControl MyAssignmentsView = new MyAssignmentsView();
		UserControl MyAccountView = new MyAccountView();
		UserControl SettingsView = new SettingsView();
        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			string content = (sender.SelectedItem as NavigationViewItem).Tag?.ToString();
			if (string.IsNullOrEmpty(content)) return;
			switch (content)
			{
				case "Start":
					sender.Header = "Projektmanager";
					NavigateTo(StartView);
					break;
				case "Projects":
					sender.Header = "Projekte";
                    NavigateTo(ProjectsView);
					break;
				case "ToDos":
					sender.Header = "Meine Aufgaben";
                    NavigateTo(MyAssignmentsView);
					break;
				case "OwnAccount":
					sender.Header = "Accounteinstellungen";
                    NavigateTo(MyAccountView);
					break;
				case "Settings":
					sender.Header = "Einstellungen";
					NavigateTo(SettingsView);
                    break;
				default:
					sender.Header = "Projektmanager";
					NavigateTo(StartView);
					break;
			}
		}

        private void NavigateTo(UserControl newControl)
        {
            if (WindowContent.Content is UserControl currentControl)
            {
                navigationStack.Push(currentControl);
            }

            WindowContent.Content = newControl;
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (navigationStack.Count > 0)
            {
                WindowContent.Content = navigationStack.Pop();
            }
        }
    }
}