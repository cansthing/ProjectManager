using ModernWpf.Controls;
using ProjectManager.ViewModel;
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
    /// Interaction logic for FilterAssignments.xaml
    /// </summary>
    public partial class FilterAssignments : ContentDialog
    {
        public FilterAssignments(MyAssignmentsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
        public FilterAssignments(ProjectsViewViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show((sender as RadioButtons).SelectedItem.ToString());
            if (DataContext is MyAssignmentsViewModel)
            {
                switch ((sender as RadioButtons).SelectedItem.ToString())
                {
                    case "":
                        break;
                    default:
                        break;
                }
                //(DataContext as MyAssignmentsViewModel).AssignmentOrder 
            }
        }
    }
}
