using ProjectManager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.DataProvider
{
    public interface IDataProvider
    {
        User CurrentUser { get; }
        Task<bool> Login(User user);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<ObservableCollection<User>> GetUsers();
        Task<User> GetUser(User user);
    }
}
