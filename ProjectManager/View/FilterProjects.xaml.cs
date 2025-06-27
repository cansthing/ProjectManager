using ModernWpf.Controls;
using ProjectManager.DataProvider;
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
    /// Interaction logic for FilterProjects.xaml
    /// </summary>
    public partial class FilterProjects : ContentDialog
    {
        private ProjectsViewViewModel ViewModel;
        public FilterProjects(ProjectsViewViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SetFilter();
        }
        private void SetFilter()
        {
            switch (radioButtons.SelectedIndex)
            {
                case 0:
                    ViewModel.ProjectFilter = ProjectFilter.No;
                    break;
                case 1:
                    ViewModel.ProjectFilter = ProjectFilter.MyProjects;
                    break;
                case 2:
                    ViewModel.ProjectFilter = ProjectFilter.ProjectFrom;
                    break;
                default:
                    ViewModel.ProjectFilter = ProjectFilter.No;
                    break;
            }
        }
    }
}
