using ProjectManager.DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Model
{
    public class Assignment
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public int ProgressPercent { get; set; }

        public override string ToString()
        {
            return Title ?? "";
        }
    }

}