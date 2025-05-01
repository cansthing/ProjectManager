using ProjectManager.Commands;
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
            set { projects = value; }
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





        public ICommand CreateProjectCommand  => new MyICommand(CreateProject);
        public ICommand EditProjectCommand => new MyICommand(EditProject);
        public ICommand SaveProjectCommand => new MyICommand(SaveProject);
        public ICommand DeleteProjectCommand => new MyICommand(DeleteProject);
        public ICommand FilterProjectsCommand => new MyICommand(FilterProjects);

        public ProjectsViewViewModel()
        {
            
        }
        private async void LoadProjects()
        {
            Projects = await ObjectRepository.DataProvider.GetProjects();
        }

        private async void CreateProject(object obj)
        {
            NewProject = new Project();
            Users = await ObjectRepository.DataProvider.GetUsers();
            var dialog = new CreateProject();
            await dialog.ShowAsync();
            LoadProjects();
        }
        private async void EditProject(object obj)
        {
            if (SelectedProject == null) return;
            NewProject = SelectedProject;
            Users = await ObjectRepository.DataProvider.GetUsers();
            var dialog = new CreateProject();
            dialog.Title = "Projekt bearbeiten";
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
            var result = MessageBox.Show("Soll das Projekt gelöscht werden?", "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes)
            {
                ObjectRepository.DataProvider.DeleteProject(SelectedProject);
            }
            LoadProjects();
        }


        private async void FilterProjects(object obj)
        {
            var dialog = new FilterProjects();
            await dialog.ShowAsync();
        }
    }
}
