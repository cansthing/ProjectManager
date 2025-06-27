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
using System.Windows;
using System.Windows.Data;
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
                if (value != null) ObjectRepository.DataProvider.UpdateAssignment(value);
                OnPropertyChanged();
            }
        }

        private int selectedAssignmentIndex = 0;
        public int SelectedAssignmentIndex
        {
            get { return selectedAssignmentIndex; }
            set { selectedAssignmentIndex = value;
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
            Assignment selected = SelectedAssignment == null ? new Assignment() { Id=0} : SelectedAssignment;
            int index = SelectedAssignmentIndex;
            MyAssignments = await ObjectRepository.DataProvider.GetAssignments(ObjectRepository.DataProvider.CurrentUser);

            int newIndex = 0;
            for (int i = 0; i < MyAssignments.Count; i++)
            {
                if (MyAssignments[i].Id == selected.Id)
                {
                    newIndex = i;
                }
            }
            SelectedAssignmentIndex = newIndex;

            await ObjectRepository.NotificationService.AllAssignments(MyAssignments);





            var view = CollectionViewSource.GetDefaultView(MyAssignments) as ICollectionView;

            if (view == null) return;

            view.GroupDescriptions.Clear();
            string filter = "";
            switch (AssignmentOrder)
            {
                case AssignmentOrder.Project:
                    filter = "Project.Title";
                    break;
                case AssignmentOrder.Priority:
                    filter = "Priority";
                    break;
                default:
                    break;
            }

            if (filter == "kein Filter")
            {
                view.Refresh();
                return;
            }

            view.GroupDescriptions.Add(new PropertyGroupDescription(filter));
            view.Refresh();
        }

        private async void OrderAssignments(object obj)
        {
            MessageBox.Show("Button wurde geklickt.");
            await FilterDialog.ShowAsync();
            LoadMyAssignments();
        }
    }
}
