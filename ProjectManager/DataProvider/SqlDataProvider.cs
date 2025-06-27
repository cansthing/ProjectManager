using ProjectManager.Helper;
using ProjectManager.Model;
using ProjectManager.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace ProjectManager.DataProvider
{
    public class SqlDataProvider : IDataProvider
    {
        private User currentUser;
        public User CurrentUser { get => currentUser; }

        public async Task<bool> CreateUser(User user)
        {
            try
            {
                user.Password = EncryptionHelper.EncryptString(user.Password);
                string query = "INSERT INTO Users (Firstname, Lastname, Email, Phone, Username, Password, IsAdmin, Image) VALUES (@Firstname, @Lastname, @Email, @Phone, @Username, @Password, @IsAdmin, @Image)";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Firstname", (object)user.Firstname ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Lastname", (object)user.Lastname ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Username", (object)user.Username ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Password", (object)user.Password ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsAdmin", (object)user.IsAdmin);
                    cmd.Parameters.AddWithValue("@Image", user.Image != null && user.imageHasBeenSet ? (object)ImageHelper.ImageSourceToBinary(user.Image) : DBNull.Value);
                    await conn.OpenAsync();
                    int a = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return a != 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Benutzers:" + ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                bool mustUpdateCurrentUser = user.Id == CurrentUser.Id;
                user.Password = EncryptionHelper.EncryptString(user.Password);
                string query = "UPDATE Users SET Firstname = @Firstname, Lastname = @Lastname, Email = @Email, Phone = @Phone, Username = @Username, Password = @Password, IsAdmin = @IsAdmin, Image = @Image WHERE Id = @Id";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
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
                    cmd.Parameters.AddWithValue("@Image", user.Image != null && user.imageHasBeenSet ? (object)ImageHelper.ImageSourceToBinary(user.Image) : DBNull.Value);
                    await conn.OpenAsync();
                    int a = await cmd.ExecuteNonQueryAsync();
                    if (mustUpdateCurrentUser)
                        currentUser = await GetUser(user.Id);
                    return a != 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren des Benutzers: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteUser(User user)
        {
            try
            {
                string query = $"DELETE FROM Users WHERE Id = {user.Id}";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    int a = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return a != 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen des Benutzers: "+ ex.Message);
                return false;
            }
        }
        public async Task<ObservableCollection<User>> GetUsers(UserFilter userFilter = UserFilter.No)
        {
            try
            {
                ObservableCollection<User> Users = new ObservableCollection<User>();
                string query;
                switch (userFilter)
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

                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
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
                        user.imageHasBeenSet = false;
                        Users.Add(user);
                    }
                    conn.Close();
                }
                return Users;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Benutzer: " + ex.Message);
                return new ObservableCollection<User>();
            }
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
            try
            {
                User fullUser = null;
                string query = $"SELECT * FROM Users WHERE Username = '{user.Username}'";

                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
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
                            Image = reader.IsDBNull(8) ? null : ImageHelper.ConvertToImageSource(reader.GetSqlBytes(8).Value)//,
                            //imageHasBeenSet = false
                        };
                        fullUser = newuser;
                    }
                }
                return fullUser;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Benutzers. Der gewünschte Benutzer scheint nicht zu existieren. "+ex.Message);
                return null;
            }
        }
        public async Task<User> GetUser(int id)
        {
            try
            {
                User fullUser = null;
                string query = $"SELECT * FROM Users WHERE Id = '{id}'";

                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
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
                            Image = reader.IsDBNull(8) ? null : ImageHelper.ConvertToImageSource(reader.GetSqlBytes(8).Value)//,
                            //imageHasBeenSet = false
                        };
                        fullUser = newuser;
                    }
                }
                return fullUser;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden des Benutzers. Der gewünschte Benutzer scheint nicht zu existieren. " + ex.Message);
                return null;
            }
        }
        public async Task<bool> CreateProject(Project project1)
        {
            MakeSureProjectsTable();
            try
            {
                string query = "INSERT INTO Projects (Title, Responsibility, Description) VALUES (@Title, @Responsibility, @Description)";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", (object)project1.Title);
                    cmd.Parameters.AddWithValue("@Responsibility", (object)project1.Responsibility.Id);
                    cmd.Parameters.AddWithValue("@Description", (object)project1.Description);
                    await conn.OpenAsync();
                    int i = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return i > 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen des Projekts. "+ex.Message);
                return false; ;
            }
}
        public async Task<bool> UpdateProject(Project project)
        {
            try
            {
                string query = $"UPDATE Projects SET [Title] = @Title, Responsibility = @Responsibility, [End] = @End, [Description] = @Description WHERE Id = @Id";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", (object)project.Id);
                    cmd.Parameters.AddWithValue("@Title", (object)project.Title);
                    cmd.Parameters.AddWithValue("@Responsibility", (object)project.Responsibility.Id);
                    cmd.Parameters.AddWithValue("@End", (project.End == DateTime.MinValue) ? DBNull.Value : (object)project.End);
                    cmd.Parameters.AddWithValue("@Description", (object)project.Description);
                    await conn.OpenAsync();
                    int i = await cmd.ExecuteNonQueryAsync();
                    return i > 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualisieren des Projekts: "+ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteProject(Project project)
        {
            try
            {
                string query = $"DELETE FROM Projects WHERE Id = '{project.Id}'";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    int i = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return i > 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen des Projekts: " + ex.Message);
                return false;
            }
        }
        public async Task<ObservableCollection<Project>> GetProjects()
        {
            return await GetProjects(ProjectFilter.No);
        }
        public async Task<ObservableCollection<Project>> GetProjects(ProjectFilter projectFilter, User user = null)
        {
            MakeSureProjectsTable();
            try
            {
                string query = "SELECT * FROM Projects ORDER BY Title ASC;";
                switch (projectFilter)
                {
                    case ProjectFilter.No:
                        query = "SELECT * FROM Projects ORDER BY Title ASC;";
                        break;
                    case ProjectFilter.MyProjects:
                        query = $"SELECT * FROM Projects WHERE Responsibility = '{currentUser.Id}' ORDER BY Title ASC;";
                        break;
                    case ProjectFilter.ProjectFrom:
                        query = $"SELECT * FROM Projects WHERE Responsibility = '{user.Id}' ORDER BY Title ASC;";
                        break;

                }

                ObservableCollection<Project> projects = new ObservableCollection<Project>();

                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        Project project = new Project()
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Responsibility = await GetUser(reader.GetInt32(2)),
                            Start = reader.GetDateTime(3),
                            End = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                            Description = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                        project.MyAssignments = await GetAssignments(CurrentUser);
                        project.Assignments = await GetAssignments(project);
                        projects.Add(project);
                    }
                }
                return projects;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Projekte: "+ex.Message);
                return new ObservableCollection<Project>();
            }
        }
        public async Task<Project> GetProject(int id)
        {
            string query = $"SELECT * FROM Projects WHERE Id = @id ORDER BY Title ASC;";
            try
            {
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var project = new Project
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Responsibility = await GetUser(reader.GetInt32(2)),
                                Start = reader.GetDateTime(3),
                                End = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                Description = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };
                            //project.MyAssignments = await GetAssignments(CurrentUser);
                            //project.Assignments = await GetAssignments(project);
                            return project;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden des Projekts: {ex.Message}");
            }
            return null;
        }
        public async Task<bool> IsMyProject(Project project)
        {
            return project.Responsibility.Id == currentUser.Id;
        }
        public async Task<bool> CreateAssignment(Assignment assignment)
        {
            try
            {
                string query = "INSERT INTO Assignments (Project, [User], Title, Description, DueDate, ProgressPercent, Priority) VALUES (@Project, @User, @Title, @Description, @DueDate, @ProgressPercent, @Priority);";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Project", assignment.Project.Id);
                    cmd.Parameters.AddWithValue("@User", assignment.User.Id);
                    cmd.Parameters.AddWithValue("@Title", assignment.Title ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", assignment.DueDate == null ? (object)DBNull.Value : assignment.DueDate);
                    cmd.Parameters.AddWithValue("@ProgressPercent", assignment.ProgressPercent);
                    cmd.Parameters.AddWithValue("@Priority", assignment.Priority.ToString() ?? (object)DBNull.Value);
                    await conn.OpenAsync();
                    int a = await cmd.ExecuteNonQueryAsync();
                    return a > 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Erstellen der Aufgabe: "+ex.Message);
                return false;
            }
        }
        public async Task<bool> UpdateAssignment(Assignment assignment)
        {
            try
            {
                string query = "UPDATE Assignments SET Project = @Project, [User] = @User, Title = @Title, Description = @Description, DueDate = @DueDate, ProgressPercent = @ProgressPercent, Priority = @Priority WHERE Id = @Id";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Project", assignment.Project.Id);
                    cmd.Parameters.AddWithValue("@User", assignment.User.Id);
                    cmd.Parameters.AddWithValue("@Title", assignment.Title ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", assignment.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", assignment.DueDate == DateTime.MinValue ? (object)DBNull.Value : assignment.DueDate);
                    cmd.Parameters.AddWithValue("@ProgressPercent", assignment.ProgressPercent == 0 ? 0 : assignment.ProgressPercent);
                    cmd.Parameters.AddWithValue("@Priority", assignment.Priority.ToString() ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id", assignment.Id);

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Aktualiseren der Aufgabe: " + ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteAssignment(Assignment assignment)
        {
            try
            {
                string query = "DELETE FROM Assignments WHERE Id = @Id";
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", assignment.Id);
                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen der Aufgabe: " + ex.Message);
                return false;
            }
        }
        public async Task<ObservableCollection<Assignment>> GetAssignments(Project project)
        {
            try
            {
                string query = "SELECT * FROM Assignments WHERE Project = @ProjectId";
                ObservableCollection<Assignment> assignments = new ObservableCollection<Assignment>();

                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectId", project.Id);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Assignment assignment = new Assignment
                            {
                                Id = reader.GetInt32(0),
                                Project = await GetProjectSafely(reader.GetInt32(1)),
                                User = await GetUserSafely(reader.GetInt32(2)),
                                Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                ProgressPercent = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                AssignDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6),
                                DueDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7),
                                Priority = ParsePriority(reader.IsDBNull(8) ? "Hoch" : reader.GetString(8))
                            };

                            assignments.Add(assignment);
                        }
                    }
                }

                return assignments;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Aufgaben: "+ex.Message);
                return new ObservableCollection<Assignment>();
            }
        }
        public async Task<ObservableCollection<Assignment>> GetAssignments(User user, AssignmentFilter assignmentFilter = AssignmentFilter.No)
        {
            try
            {
                string query = "SELECT * FROM Assignments WHERE [User] = @UserId";

                if (assignmentFilter != AssignmentFilter.No)
                {
                    query += " AND Priority = @Priority";
                }

                ObservableCollection<Assignment> assignments = new ObservableCollection<Assignment>();

                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", user.Id);

                    if (assignmentFilter != AssignmentFilter.No)
                    {
                        cmd.Parameters.AddWithValue("@Priority", assignmentFilter.ToString());
                    }

                    try
                    {
                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Assignment assignment = new Assignment
                                {
                                    Id = reader.GetInt32(0),
                                    Project = await GetProjectSafely(reader.GetInt32(1)),
                                    User = await GetUserSafely(reader.GetInt32(2)),
                                    Title = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    ProgressPercent = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                    AssignDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6),
                                    DueDate = reader.IsDBNull(7) ? DateTime.MinValue : reader.GetDateTime(7),
                                    Priority = ParsePriority(reader.IsDBNull(8) ? "Hoch" : reader.GetString(8))
                                };

                                assignments.Add(assignment);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fehler beim Laden GetAssignments(User...): {ex.Message}");
                    }
                }

                return assignments;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Aufgabe: " + ex.Message);
                return new ObservableCollection<Assignment>();
            }
        }
        private async Task<Project> GetProjectSafely(int projectId)
        {
            return projectId > 0 ? await GetProject(projectId) : null;
        }
        private async Task<User> GetUserSafely(int userId)
        {
            return userId > 0 ? await GetUser(userId) : null;
        }
        private Priority ParsePriority(string priorityString)
        {
            switch (priorityString)
            {
                case "Hoch":
                    return Priority.Hoch;
                case "Mittel":
                    return Priority.Mittel;
                case "Niedrig":
                    return Priority.Niedrig;
                default:
                    return Priority.Mittel;
            }
            //return Enum.TryParse(priorityString, out Priority priority) ? priority : Priority.Hoch;
        }
        private void MakeSureProjectsTable()
        {
            string query = "CREATE TABLE [dbo].[Projects] ([Id] INT IDENTITY (1, 1) NOT NULL,[Title] VARCHAR (50)  NULL,[Responsibility] INT NULL,[Start] DATETIME DEFAULT (CONVERT([date],getdate())) NULL,[End] DATETIME NULL,[Description] VARCHAR (255) NULL,CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id] ASC));";
            try
            {
                using (SqlConnection conn = new SqlConnection(App.DBConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                };
            }
            catch (Exception) { }
        }
    }
}