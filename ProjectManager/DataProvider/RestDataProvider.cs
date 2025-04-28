using ProjectManager.Helper;
using ProjectManager.Model;
using ProjectManager.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace ProjectManager.DataProvider
{
    public class RestDataProvider : IDataProvider
    {
        private User currentUser;
        public User CurrentUser { get => currentUser; }
        public async Task<bool> CreateUser(User user)
        {
            user.Password = EncryptionHelper.EncryptString(user.Password);
            string query = "INSERT INTO Users (Firstname, Lastname, Email, Phone, Username, Password) VALUES (@Firstname, @Lastname, @Email, @Phone, @Username, @Password)";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Firstname", (object)user.Firstname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lastname", (object)user.Lastname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Username", (object)user.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", (object)user.Password ?? DBNull.Value);
                await conn.OpenAsync();
                int a = await cmd.ExecuteNonQueryAsync();
                return a != 0;
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            user.Password = EncryptionHelper.EncryptString(user.Password);
            string query = "UPDATE Users (Firstname = @Firstname, Lastname = @Lastname, Email = @Email, Phone = @Phone, Username = @Username, Password = @Password) WHERE Id = @Id";
            using (SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("Id", (object)user.Id);

                cmd.Parameters.AddWithValue("@Firstname", (object)user.Firstname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lastname", (object)user.Lastname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Username", (object)user.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", (object)user.Password ?? DBNull.Value);
                await conn.OpenAsync();
                int a = await cmd.ExecuteNonQueryAsync();
                return a != 0;
            }
        }
        public async Task<bool> DeleteUser(User user)
        {
            string query = "";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                int a = await cmd.ExecuteNonQueryAsync();
                return a != 0;
            }
        }
        public async Task<ObservableCollection<User>> GetUsers()
        {
            ObservableCollection<User> Users = new ObservableCollection<User>();
            string query = "SELECT * FROM Users";

            using (SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync()) 
                {
                    while (reader.Read())
                    {
                        User user = new User()
                        {
                            Id = reader.GetInt32(0),
                            Firstname = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Lastname = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Username = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Password = reader.IsDBNull(6) ? null : EncryptionHelper.DecryptString(reader.GetString(6))
                        };
                        Users.Add(user);
                    }
                } 
            } 

            return Users;
        }
        public async Task<bool> Login(User user)
        {
            try
            {
                User fullUser = await GetUser(user);
                if (fullUser == null)
                    return false;
                if (user.Password == fullUser.Password)
                {
                    currentUser = fullUser;
                    ObjectRepository.AppConfiguration.CurrentUser = fullUser;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        public async Task<User> GetUser(User user)
        {
            User fullUser = null;
            string query = $"SELECT * FROM Users WHERE Username = '{user.Username}'";

            using (SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User newuser = new User()
                    {
                        Id = reader.GetInt32(0),
                        Firstname = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Lastname = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Username = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Password = reader.IsDBNull(6) ? null : EncryptionHelper.DecryptString(reader.GetString(6))
                    };
                    fullUser = newuser;
                }
            }
            return fullUser;
        }
    }
}
