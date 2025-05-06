using ProjectManager.Helper;
using ProjectManager.Model;
using ProjectManager.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace ProjectManager.DataProvider
{
    public class RestDataProvider : IDataProvider
    {
        private User currentUser;
        public User CurrentUser { get => currentUser; }

        /// <summary>
        /// Creates a new User in the database.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> CreateUser(User user)
        {
            user.Password = EncryptionHelper.EncryptString(user.Password);
            string query = "INSERT INTO Users (Firstname, Lastname, Email, Phone, Username, Password, IsAdmin, Image) VALUES (@Firstname, @Lastname, @Email, @Phone, @Username, @Password, @IsAdmin, @Image)";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Firstname", (object)user.Firstname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Lastname", (object)user.Lastname ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Username", (object)user.Username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Password", (object)user.Password ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsAdmin", (object)user.IsAdmin);
                cmd.Parameters.AddWithValue("@Image", (object)user.Image != null ? (object)ImageHelper.ImageSourceToBinary(user.Image) : DBNull.Value); 
                await conn.OpenAsync();
                int a = await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return a != 0;
            }
        }

        /// <summary>
        /// Updates an existing user in the database. The user is recognized by Id.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUser(User user)
        {
            bool mustUpdateCurrentUser = user.Id == CurrentUser.Id;
            user.Password = EncryptionHelper.EncryptString(user.Password);
            string query = "UPDATE Users SET Firstname = @Firstname, Lastname = @Lastname, Email = @Email, Phone = @Phone, Username = @Username, Password = @Password, IsAdmin = @IsAdmin, Image = @Image WHERE Id = @Id";
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
                cmd.Parameters.AddWithValue("@IsAdmin", (object)user.IsAdmin);
                cmd.Parameters.AddWithValue("@Image", user.Image != null ? (object)ImageHelper.ImageSourceToBinary(user.Image) : DBNull.Value); 
                await conn.OpenAsync();
                int a = await cmd.ExecuteNonQueryAsync();
                currentUser = await GetUser(user.Id);
                return a != 0;
            }
        }

        /// <summary>
        /// Deletes an user permanently.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(User user)
        {
            string query = $"DELETE FROM Users WHERE Id = {user.Id}";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                int a = await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return a != 0;
            }
        }

        /// <summary>
        /// Returns All Users saved in the database. You can filter by Admins or Users. By default there is no filter.
        /// </summary>
        /// <param name="userFilter"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<User>> GetUsers(UserFilter userFilter = UserFilter.No)
        {
            ObservableCollection<User> Users = new ObservableCollection<User>();
            string query;
            switch(userFilter)
            {
                case UserFilter.No:
                    query = "SELECT * FROM Users;";
                    break;
                case UserFilter.Admin:
                    query = "SELECT * FROM Users WHERE IsAdmin = TRUE;";
                    break;
                case UserFilter.User:
                    query = "SELECT * FROM Users WHERE IsAdmin = FALSE;";
                    break;
                default:
                    query = "SELECT * FROM Users";
                    break;
            }

            using (SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                var reader = cmd.ExecuteReader();
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
                        Password = reader.IsDBNull(6) ? null : EncryptionHelper.DecryptString(reader.GetString(6)),
                        IsAdmin = reader.GetBoolean(7),
                        Image = reader.IsDBNull(8) ? null : ImageHelper.ConvertToImageSource(reader.GetSqlBytes(8).Value)
                    };
                    Users.Add(user);
                }
                conn.Close();
            } 

            return Users;
        }

        /// <summary>
        /// If the username and password is correct, returns true.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the full User of the givven User recognized by username.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
                        Password = reader.IsDBNull(6) ? null : EncryptionHelper.DecryptString(reader.GetString(6)),
                        IsAdmin = reader.GetBoolean(7),
                        Image = reader.IsDBNull(8) ? null : ImageHelper.ConvertToImageSource(reader.GetSqlBytes(8).Value)
                    };
                    fullUser = newuser;
                }
                conn.Close();
            }
            return fullUser;
        }

        public async Task<User> GetUser(int id)
        {
            User fullUser = null;
            string query = $"SELECT * FROM Users WHERE Id = '{id}'";

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
                        Password = reader.IsDBNull(6) ? null : EncryptionHelper.DecryptString(reader.GetString(6)),
                        IsAdmin = reader.GetBoolean(7),
                        Image = reader.IsDBNull(8) ? null : ImageHelper.ConvertToImageSource(reader.GetSqlBytes(8).Value)
                    };
                    fullUser = newuser;
                }
                conn.Close();
            }
            return fullUser;
        }

        public async Task<bool> CreateProject(Project project1)
        {
            string query = "INSERT INTO Projects (Title, Responsibility, Description) VALUES (@Title, @Responsibility, @Description)";
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Title", (object)project1.Title);
                cmd.Parameters.AddWithValue("@Responsibility", (object)project1.Responsibility.Id);
                cmd.Parameters.AddWithValue("@Description", (object)project1.Description);
                await conn.OpenAsync();
                int i= await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return i > 0;
            }
        }

        public async Task<bool> UpdateProject(Project project)
        {
            string query = $"UPDATE Projects SET Title = @Title, Responsibility = @Responsibility, End = @End, Description = @Description WHERE Id = @Id";
            using(SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", (object)project.Id);
                cmd.Parameters.AddWithValue("@Title", (object)project.Title);
                cmd.Parameters.AddWithValue("@Responsibility", (object)project.Responsibility.Id);
                cmd.Parameters.AddWithValue("@End", (object)project.End);
                cmd.Parameters.AddWithValue("@Description", (object)project.Description);
                await conn.OpenAsync();
                int i= await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return i > 0;
            }
        }

        public async Task<bool> DeleteProject(Project project)
        {
            string query = $"DELETE FROM Projects WHERE Id = '{project.Id}'";
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                int i= await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return i > 0;
            }
        }
        public async Task<ObservableCollection<Project>> GetProjects()
        {
            return await GetProjects(ProjectFilter.No);
        }
        public async Task<ObservableCollection<Project>> GetProjects(ProjectFilter projectFilter, User user = null)
        {
            string query = "SELECT * FROM Projects";
            switch(projectFilter)
            {
                case ProjectFilter.No:
                    query = "SELECT * FROM Projects;";
                    break;
                case ProjectFilter.MyProjects:
                    query = $"SELECT * FROM Projects WHERE Responsibility = '{currentUser.Id}'";
                    break;
                case ProjectFilter.ProjectFrom:
                    query = $"SELECT * FROM Projects WHERE Responsibility = '{user.Id}'";
                    break;

            }
            ObservableCollection<Project> projects = new ObservableCollection<Project>();
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Project project = new Project()
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                        Responsibility = await GetUser(reader.GetInt32(2)),
                        Start = reader.GetDateTime(3),
                        End = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                        Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                        MyAssignments = await GetAssignments(CurrentUser)
                    };
                    projects.Add(project);
                }
            }
            return projects;
        }
        public async Task<Project> GetProject(int id)
        {
            string query = $"SELECT * FROM Projects WHERE Id = {id}";
            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Project
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Responsibility = await GetUser(reader.GetInt32(2)),
                            Start = reader.GetDateTime(3),
                            End = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                            Description = reader.IsDBNull(5) ? null : reader.GetString(5),
                            MyAssignments = await GetAssignments(CurrentUser)
                        };
                    }
                }
            }
            return null; 
        }
        public async Task<bool> IsMyProject(Project project)
        {
            await Task.Delay(0);
            return project.Responsibility.Id == currentUser.Id;
        }

        public async Task<bool> CreateAssignment(Assignment assignment)
        {
            string query = "INSERT INTO Assignments (Project, User, Title, Description, DueDate, ProgressPercent) VALUES (@Project, @User, @Title, @Description, @DueDate, ProgressPercent)";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Project", assignment.Project.Id);
                cmd.Parameters.AddWithValue("@User", assignment.User.Id);
                cmd.Parameters.AddWithValue("@Title", assignment.Title ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DueDate", assignment.DueDate == null ? (object)DBNull.Value : assignment.DueDate);
                cmd.Parameters.AddWithValue("@ProgressPercent", assignment.ProgressPercent);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> UpdateAssignment(Assignment assignment)
        {
            string query = "UPDATE Assignments SET ";
            using (SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Project", assignment.Project.Id);
                cmd.Parameters.AddWithValue("@User", assignment.User.Id);
                cmd.Parameters.AddWithValue("@Title", assignment.Title ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DueDate", assignment.DueDate == null ? (object)DBNull.Value : assignment.DueDate);
                cmd.Parameters.AddWithValue("@ProgressPercent", assignment.ProgressPercent);
                cmd.Parameters.AddWithValue("@Id", assignment.Id);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> DeleteAssignment(Assignment assignment)
        {
            string query = "DELETE FORM Assignemnts WHERE Id = @Id";
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", assignment.Id);
                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }
        public async Task<ObservableCollection<Assignment>> GetAssignments(Project project)
        {
            string query = $"SELECT * FROM Assignments WHERE Project = {project.Id}";
            ObservableCollection<Assignment> assignments = new ObservableCollection<Assignment>();
            using (SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Assignment assignment = new Assignment()
                    {
                        Id = reader.GetInt32(0),
                        Project = await GetProject(reader.GetInt32(1)),
                        User = await GetUser(reader.GetInt32(2)),
                        Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ProgressPercent = reader.GetInt32(5),
                        AssignDate = reader.GetDateTime(6),
                        DueDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7),
                        Priority = (Priority)Enum.Parse(typeof(Priority), reader.GetString(8))
                    };
                    assignments.Add(assignment);
                }
            }
            return assignments;
        }
        public async Task<ObservableCollection<Assignment>> GetAssignments(User user, AssignmentFilter assignmentFilter = AssignmentFilter.No)
        {
            string query = "SELECT * FROM Assignments";
            switch (assignmentFilter)
            {
                case AssignmentFilter.No:
                    query = $"SELECT * FROM Assignments WHERE User = {user.Id}";
                    break;
                case AssignmentFilter.Priorität_Hoch:
                    query = $"SELECT * FROM Assignments WHERE User = {user.Id} AND Priority = {Priority.Hoch.ToString()}";
                    break;
                case AssignmentFilter.Priorität_Mittel:
                    query = $"SELECT * FROM Assignments WHERE User = {user.Id} AND Priority = {Priority.Mittel.ToString()}";
                    break;
                case AssignmentFilter.Priorität_Niedrig:
                    query = $"SELECT * FROM Assignments WHERE User = {user.Id} AND Priority = {Priority.Niedrig.ToString()}";
                    break;
                default:
                    query = $"SELECT * FROM Assignments WHERE User = {user.Id}";
                    break;
            }
            ObservableCollection<Assignment> assignments = new ObservableCollection<Assignment>();
            using(SqlConnection conn = new SqlConnection(Settings.Default.DBConnectionString))
            using(SqlCommand cmd = new SqlCommand(query, conn))
            {
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    Assignment assignment = new Assignment()
                    {
                        Id = reader.GetInt32(0),
                        Project = await GetProject(reader.GetInt32(1)),
                        User = await GetUser(reader.GetInt32(2)),
                        Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ProgressPercent = reader.GetInt32(5),
                        AssignDate = reader.GetDateTime(6),
                        DueDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7),
                        Priority = (Priority)Enum.Parse(typeof(Priority), reader.GetString(8))
                    };
                    assignments.Add(assignment);
                }
            }
            return assignments;
        }
    }
}