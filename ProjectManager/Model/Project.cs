using ProjectManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public User Responsibility { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Assignment> MyAssignments { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
