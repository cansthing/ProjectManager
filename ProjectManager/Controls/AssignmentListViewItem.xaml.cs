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

namespace ProjectManager.Controls
{
    /// <summary>
    /// Interaction logic for AssignmentListViewItem.xaml
    /// </summary>
    public partial class AssignmentListViewItem : UserControl
    {


        public Assignment Assignment
        {
            get { return (Assignment)GetValue(AssignmentProperty); }
            set { SetValue(AssignmentProperty, value); }
        }

        public static readonly DependencyProperty AssignmentProperty = DependencyProperty.Register("Assignment", typeof(Assignment), typeof(AssignmentListViewItem), new PropertyMetadata(new Assignment(), SetText));

        private static void SetText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AssignmentListViewItem control = d as AssignmentListViewItem;
            if(control != null)
            {
                control.tb_title.Text = (e.NewValue as Assignment).Title;
                control.lbl_duedate.Content = (e.NewValue as Assignment).DueDate.ToString("dd.MM.yyyy");
                control.border_priority.Background = GetColor(e.NewValue as Assignment);
            }
        }
        private static Brush GetColor(Assignment assignment)
        {
            string priority = assignment.Priority.ToString();
            switch (priority)
            {
                case "Hoch":
                    return Brushes.Red;
                case "Mittel":
                    return Brushes.Orange;
                case "Niedrig":
                    return Brushes.Green;
                default:
                    return Brushes.Gray;
            }
        }

        public AssignmentListViewItem()
        {
            InitializeComponent();
        }
    }
}
