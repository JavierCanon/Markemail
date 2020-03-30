using System;
using System.Windows;

namespace EmailClient.Dialogs
{
    /// <summary>
    /// Dialog window for adding a new User
    /// </summary>
    public partial class NewUserDialog : Window
    {
        /// <summary>
        /// Gets the name of the new user.
        /// </summary>
        public string UserName => nameField.Text;

        /// <summary>
        /// Gets the birth date of the new user.
        /// </summary>
        public DateTime BirthDate => calendar.DisplayDate;

        /// <summary>
        /// Initializes a new dialog window
        /// </summary>
        public NewUserDialog()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// On pressing the ok button we confirm the data of the new user.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// On pressing the cancel button, we cancel the new user's creational process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
