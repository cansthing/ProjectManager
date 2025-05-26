using ProjectManager.DataProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Model
{
    public class Assignment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int Id { get; set; }
        public Project Project { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        private int progressPercent;
        public int ProgressPercent { get => progressPercent; set { progressPercent = value; OnPropertyChanged(); ObjectRepository.DataProvider.UpdateAssignment(this); OnPropertyChanged(); } }

        public override string ToString()
        {
            return Title ?? "";
        }
    }

}