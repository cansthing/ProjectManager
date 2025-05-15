using ProjectManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DataProvider
{
    public interface ISendNotification
    {
        Task<bool> IsValid(User user);
        Task<bool> Send(User to, string subject, StringBuilder content);
        Task<bool> AssignmentCreated(Assignment assignment);
        Task<bool> AssignmentUpdated(Assignment assignment);
        Task<bool> AssignmentDeleted(Assignment assignment);
        Task AllAssignments(ObservableCollection<Assignment> assignments);
    }
}
