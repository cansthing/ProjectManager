using MimeKit.Cryptography;
using ProjectManager.Commands;
using ProjectManager.DataProvider;
using ProjectManager.Model;
using ProjectManager.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectManager.ViewModel
{
    public class ProjectsViewViewModel : INotifyPropertyChanged
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

        private ObservableCollection<Project> projects;
        public ObservableCollection<Project> Projects
        {
            get { return projects; }
            set { projects = value;
                OnPropertyChanged();
            }
        }

        private Project newProject;
        public Project NewProject
        {
            get { return newProject; }
            set { newProject = value;
                OnPropertyChanged();
            }
        }

        private Project selectedProject;
        public Project SelectedProject
        {
            get { return selectedProject; }
            set { selectedProject = value;
                OnPropertyChanged();
            }
        }

        private int selectedProjectIndex = 0;
        public int SelectedProjectIndex
        {
            get { return selectedProjectIndex; }
            set { selectedProjectIndex = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set { users = value; }
        }

        private User selectedFilterUser;
        public User SelectedFilterUser
        {
            get { return selectedFilterUser; }
            set { selectedFilterUser = value;
                OnPropertyChanged();
            }
        }

        private ProjectFilter projectFilter = ProjectFilter.No;
        public ProjectFilter ProjectFilter
        {
            get { return projectFilter; }
            set { projectFilter = value;
                OnPropertyChanged();
            }
        }

        public FilterProjects FilterDialog { get; set; }

        private Assignment newAssignment;
        public Assignment NewAssignment
        {
            get { return newAssignment; }
            set { newAssignment = value;
                OnPropertyChanged();
            }
        }
        public Array Priorities => Enum.GetValues(typeof(Priority));

        private Assignment selectedAssignment;
        public Assignment SelectedAssignment
        {
            get { return selectedAssignment; }
            set { selectedAssignment = value;
                OnPropertyChanged();
            }
        }

        

        public ICommand CreateProjectCommand  => new MyICommand(CreateProject);
        public ICommand EditProjectCommand => new MyICommand(EditProject);
        public ICommand SaveProjectCommand => new MyICommand(SaveProject);
        public ICommand DeleteProjectCommand => new MyICommand(DeleteProject);
        public ICommand FilterProjectsCommand => new MyICommand(FilterProjects);

        public ICommand CreateAssignmentCommand => new MyICommand(CreateAssignmentC);
        public ICommand EditAssignmentCommand => new MyICommand(EditAssignment);
        public ICommand SaveAssignmentCommand => new MyICommand(SaveAssignment);
        public ICommand DeleteAssignmentCommand => new MyICommand(DeleteAssignment);


        public ProjectsViewViewModel()
        {
            NewProject = new Project();
            FilterDialog =  new FilterProjects(this);
            NewAssignment = new Assignment();
            LoadProjects();
        }

        private async void LoadProjects(ProjectFilter filter = ProjectFilter.No)
        {
            int index = SelectedProjectIndex;
            Project selected = SelectedProject;
            switch (filter)
            {
                case ProjectFilter.No:
                    Projects = await ObjectRepository.DataProvider.GetProjects();
                    break;
                case ProjectFilter.MyProjects:
                    Projects = await ObjectRepository.DataProvider.GetProjects(filter);
                    break;
                case ProjectFilter.ProjectFrom:
                    Projects = await ObjectRepository.DataProvider.GetProjects(filter, SelectedFilterUser);
                 break;
            }
            if (selected == null) return;
            int newIndex = 0;
            for (int i = 0; i < Projects.Count; i++)
            {
                if (Projects[i].Id == selected.Id)
                {
                    newIndex = i;
                }
            }
            SelectedProjectIndex = newIndex;
            OnPropertyChanged(nameof(SelectedProjectIndex));
        }
        private async void LoadUsers()
        {
            Users = await ObjectRepository.DataProvider.GetUsers();
        }
        private async void CreateProject(object obj)
        {
            NewProject = new Project();
            LoadUsers();
            var dialog = new CreateProject(this);
            await dialog.ShowAsync();
            LoadProjects();
        }
        private async void EditProject(object obj)
        {
            if (SelectedProject == null) return;
            NewProject = SelectedProject;
            LoadUsers();
            var dialog = new CreateProject(this)
            {
                Title = "Projekt bearbeiten"
            };
            await dialog.ShowAsync();
            LoadProjects();
        }
        private async void SaveProject(object obj)
        {
            if(NewProject.Id == 0)
            {
                await ObjectRepository.DataProvider.CreateProject(NewProject);
            }
            else
            {
                var v = ObjectRepository.DataProvider.UpdateProject(NewProject);
                Console.WriteLine(v.ToString());
            }
            LoadProjects();
        }
        private void DeleteProject(object obj)
        {
            if(SelectedProject == null) return;
            var result = MessageBox.Show("Soll das Projekt gelöscht werden?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                ObjectRepository.DataProvider.DeleteProject(SelectedProject);
            }
            LoadProjects(ProjectFilter);
        }
        private async void FilterProjects(object obj)
        {
            LoadUsers();
            await FilterDialog.ShowAsync();
            LoadProjects(ProjectFilter);
        }
        private async void CreateAssignmentC(object obj)
        {
            LoadUsers();
            NewAssignment.AssignDate = DateTime.Now;
            NewAssignment.Project = SelectedProject;
            var dialog = new CreateAssignment(this);
            await dialog.ShowAsync();
            NewAssignment.AssignDate = DateTime.Now;
        }
        private async void EditAssignment(object obj)
        {
            NewAssignment = SelectedAssignment;
            var dialog = new CreateAssignment(this)
            {
                Title = "Aufgabe bearbeiten"
            };
            await dialog.ShowAsync();
        }
        private async void SaveAssignment(object obj)
        {
            if(SelectedProject == null) return;
            if (NewAssignment == null) return;
            if(NewAssignment.Id != 0)
            {
                if(!await ObjectRepository.DataProvider.UpdateAssignment(NewAssignment))
                {
                    MessageBox.Show("Updaten der Aufgabe fehlgeschalgen");
                }
                else
                {
                    await ObjectRepository.NotificationService.AssignmentUpdated(NewAssignment);
                }
            }
            else
            {
                if(!await ObjectRepository.DataProvider.CreateAssignment(NewAssignment))
                {
                    MessageBox.Show("Erstellen der Aufgabe fehlgeschalgen");
                }
                else
                {
                    await ObjectRepository.NotificationService.AssignmentCreated(NewAssignment);
                }
            }
            NewAssignment = new Assignment();
            LoadProjects();
        }
        private void DeleteAssignment(object obj)
        {
            var selected = SelectedAssignment;
            if (selected != null)
                ObjectRepository.DataProvider.DeleteAssignment(selected);
            LoadProjects();
        }
    }
}