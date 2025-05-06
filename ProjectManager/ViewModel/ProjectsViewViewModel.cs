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




        public ICommand CreateProjectCommand  => new MyICommand(CreateProject);
        public ICommand EditProjectCommand => new MyICommand(EditProject);
        public ICommand SaveProjectCommand => new MyICommand(SaveProject);
        public ICommand DeleteProjectCommand => new MyICommand(DeleteProject);
        public ICommand FilterProjectsCommand => new MyICommand(FilterProjects);

        public ProjectsViewViewModel()
        {
            NewProject = new Project();
            FilterDialog =  new FilterProjects(this);
            LoadProjects();
        }
        private async void LoadProjects(ProjectFilter filter = ProjectFilter.No)
        {
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
        }

        private async void CreateProject(object obj)
        {
            NewProject = new Project();
            Users = await ObjectRepository.DataProvider.GetUsers();
            var dialog = new CreateProject(this);
            await dialog.ShowAsync();
            LoadProjects();
        }
        private async void EditProject(object obj)
        {
            if (SelectedProject == null) return;
            NewProject = SelectedProject;
            Users = await ObjectRepository.DataProvider.GetUsers();
            var dialog = new CreateProject(this)
            {
                Title = "Projekt bearbeiten"
            };
            await dialog.ShowAsync();
            LoadProjects();
        }


        private void SaveProject(object obj)
        {
            if(NewProject.Id == 0)
            {
                ObjectRepository.DataProvider.CreateProject(NewProject);
            }
            else
            {
                ObjectRepository.DataProvider.UpdateProject(NewProject);
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
            Users = await ObjectRepository.DataProvider.GetUsers();
            await FilterDialog.ShowAsync();
            LoadProjects(ProjectFilter);
        }
    }
}