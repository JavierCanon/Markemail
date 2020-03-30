using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EmailClient.ViewModels;
using EmailClient.Core.Model;

namespace EmailClient.Dialogs
{
    /// <summary>
    /// Switch / Select user dialog window
    /// </summary>
    public partial class SwitchUserDialog : Window
    {
        private MainViewModel _mvm;

        /// <summary>
        /// Gets the selected User.
        /// </summary>
        public User SelectedUser => (User)userList.SelectedItem;

        /// <summary>
        /// initializes a switch user dialog window
        /// </summary>
        /// <param name="mvm">Refference of the MainViewModel object</param>
        public SwitchUserDialog(MainViewModel mvm)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _mvm = mvm;
            userList.ItemsSource = _mvm.Users;
        }

        /// <summary>
        /// On pressing the ok button we confirm the selected user.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = userList != null ? true : false;
        }

        /// <summary>
        /// On pressing the cancel button, we cancel the user selection process.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
