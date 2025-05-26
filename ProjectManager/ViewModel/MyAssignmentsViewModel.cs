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
using System.Windows.Input;

namespace ProjectManager.ViewModel
{
    public class MyAssignmentsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Assignment> myAssignments;
        public ObservableCollection<Assignment> MyAssignments
        {
            get { return myAssignments; }
            set { myAssignments = value;
                OnPropertyChanged();
            }
        }

        private Assignment selectedAssignment;
        public Assignment SelectedAssignment
        {
            get { return selectedAssignment; }
            set { selectedAssignment = value;
                ObjectRepository.DataProvider.UpdateAssignment(value);
                OnPropertyChanged();
            }
        }
        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value;
                OnPropertyChanged();
            }
        }
        public int ProgressPercent
        {
            get 
            {
                int value;
                value = SelectedAssignment == null ? 0 : SelectedAssignment.ProgressPercent;
                return value; 
            }
            set 
            {
                if(SelectedAssignment != null)
                {
                    SelectedAssignment.ProgressPercent = value;
                }
                ObjectRepository.DataProvider.UpdateAssignment(SelectedAssignment);
                OnPropertyChanged();
                LoadMyAssignments();
                OnPropertyChanged();
            }
        }



        private AssignmentOrder order = AssignmentOrder.Priority;
        public AssignmentOrder AssignmentOrder
        {
            get { return order; }
            set { order = value;
                OnPropertyChanged();
            }
        }

        public FilterAssignments FilterDialog { get; set; }
        public ICommand OrderAssignmentsCommand => new MyICommand(OrderAssignments);


        public MyAssignmentsViewModel()
        {
            FilterDialog = new FilterAssignments(this);
            LoadMyAssignments();
        }
        public async void LoadMyAssignments()
        {
            int selected;
            selected = SelectedAssignment == null ? 0 : SelectedAssignment.Id;

            MyAssignments = await ObjectRepository.DataProvider.GetAssignments(ObjectRepository.DataProvider.CurrentUser);
            for(int i=0;i<MyAssignments.Count;i++)
            {
                if(MyAssignments[i].Id == selected)
                {
                    SelectedIndex = i; break;
                }
            }

            await ObjectRepository.NotificationService.AllAssignments(MyAssignments);
        }

        private async void OrderAssignments(object obj)
        {
            await FilterDialog.ShowAsync();
            LoadMyAssignments();
        }
    }
}
