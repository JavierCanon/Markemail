using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using EmailClient.Core.Model;
using EmailClient.Core.Service;

namespace EmailClient.ViewModels
{
    /// <summary>
    /// This class creates a connection between the UI and the backend.
    /// </summary>
    public class MainViewModel
    {
        public TextBlock DwnldStatus { get; private set; }
    
        /// <summary>
        /// Gets, sets the current user of the application
        /// </summary>
        public User CurrentUser { get; set; }

        /// <summary>
        /// Gets the users of the program
        /// </summary>
        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();

        /// <summary>
        /// Initializes the MainViewModel
        /// </summary>
        public MainViewModel()
        {
            var serializedUsers = Directory.GetFiles(BackupHandler.root, "*.dat");
            foreach (var serializedUser in serializedUsers)
            {
                Users.Add(User.Deserialize(serializedUser));
            }
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="name">Name of the user</param>
        public void AddUser(string name, DateTime birthDate)
        {
            Users.Add(new User(name, birthDate));
        }

        /// <summary>
        /// Adds a new user (overload)
        /// </summary>
        /// <param name="user">New user</param>
        public void AddUser(User user)
        {
            Users.Add(user);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(User user)
        {
            Users.Remove(user);
            BackupHandler.DeleteBackup(user);
        }
    }
}

