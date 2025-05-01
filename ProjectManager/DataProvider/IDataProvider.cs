using ProjectManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProjectManager.DataProvider
{
    public interface IDataProvider
    {
        User CurrentUser { get; }
        Task<bool> Login(User user);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<ObservableCollection<User>> GetUsers(UserFilter userFilter = UserFilter.No);
        Task<User> GetUser(User user);
        Task<User> GetUser(int id);


        Task<bool> CreateProject(Project project);
        Task<bool> UpdateProject(Project project);
        Task<bool> DeleteProject(Project project);
        Task<ObservableCollection<Project>> GetProjects(ProjectFilter projectFilter = ProjectFilter.No);
        Task<bool> IsMyProject(Project project);


        Task<bool> CreateAssignment(Assignment assignment);
        Task<bool> UpdateAssignment(Assignment assignment);
        Task<bool> DeleteAssignment(Assignment assignment);
        Task<ObservableCollection<Assignment>> GetMyAssignments(User user, AssignmentFilter assignmentFilter = AssignmentFilter.No);
    }
    public enum UserFilter
    {
        No,
        Admin,
        User,
        Active,
        Inactive
    }
    public enum ProjectFilter
    {
        No,
        MyProjects
    }
    public enum AssignmentFilter
    {
        No,
        Priorität_Hoch
    }
}