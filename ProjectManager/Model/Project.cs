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
        public ObservableCollection<Assignment> Assignments { get; set; }
        public ObservableCollection<Assignment> MyAssignments { get; set; }
        public int Progress
        {
            get
            {
                int S = 0;
                int W = 0;
                if (Assignments == null) return 0;
                foreach (var ass in Assignments)
                {
                    S += ass.ProgressPercent * PriorityToInt(ass.Priority.ToString());
                    W += PriorityToInt(ass.Priority.ToString());
                }
                if (W != 0)
                    return S / W;
                return 0;
            }
        }
        private int PriorityToInt(string priority)
        {
            switch (priority)
            {
                case "Hoch":
                    return 3;
                case "Mittel":
                    return 2;
                case "Niedrig":
                    return 1;
                default:
                    return 0;
            }
        }
        public override string ToString()
        {
            return Title;
        }
    }
}
