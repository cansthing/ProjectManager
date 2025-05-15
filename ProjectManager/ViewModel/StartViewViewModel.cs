using ProjectManager.Model;
using ProjectManager.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.ViewModel
{
    public class StartViewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool CanCurrentUser
        {
            get => ObjectRepository.DataProvider.CurrentUser.IsAdmin;
        }
        public User User
        {
            get => ObjectRepository.DataProvider.CurrentUser;
        }
        public int AssCOunt
        {
            get => LoadAssignmentCount();
        }

        public StartViewViewModel()
        {
            
        }
        private int LoadAssignmentCount()
        {
            string query = $"SELECT COUNT(*) FROM Assignments WHERE [User]={ObjectRepository.DataProvider.CurrentUser.Id} AND ProgressPercent < 100;";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return reader.GetInt32(0);
                }
                return 0;
            }
        }
    }
}
