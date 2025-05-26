using ProjectManager.Model;
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
    /// Interaction logic for AssignmentView.xaml
    /// </summary>
    public partial class AssignmentView : UserControl
    {


        public Assignment Assignment
        {
            get { return (Assignment)GetValue(SelectedAssignmentProperty); }
            set { SetValue(SelectedAssignmentProperty, value); }
        }

        public static readonly DependencyProperty SelectedAssignmentProperty = DependencyProperty.Register("Assignment", typeof(Assignment), typeof(AssignmentView), new PropertyMetadata(new Assignment()));



        public AssignmentView()
        {
            InitializeComponent();
        }
    }
}
