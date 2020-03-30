using EmailClient.Core.Model;
using EmailClient.ViewModels;
using MailKit;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
 
namespace EmailClient.Dialogs
{
    /// <summary>
    /// Interaction logic for ViewEmailDialog.xaml
    /// </summary>
    public partial class ViewEmailDialog : Window
    {
        private readonly MainViewModel _mvm;
        private readonly Email _email;

        /// <summary>
        /// Gets the sender of the email
        /// </summary>
        public string From => fromField.Text;

        /// <summary>
        /// Gets the subject of the email
        /// </summary>
        public string Subject => subjectField.Text;

        /// <summary>
        /// Gets the message of the email
        /// </summary>
        public string Message => messageField.Text;

        /// <summary>
        /// Initializes ViewEmailDialog window
        /// </summary>
        /// <param name="email">The email we would like to view</param>
        public ViewEmailDialog(MainViewModel mvm, Email email)
        {
            InitializeComponent();
            _mvm = mvm;
            _email = email;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            fromField.Text = email.SenderAddress;
            subjectField.Text = email.Subject;
            messageField.Text = email.Message;
        }

        /// <summary>
        /// Clicking on the close button will close the window
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Clicking on the delete button will delete the email from the IMAP server
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var message = "Do you really want to delete the current email?";
            var result = MessageBox.Show(message, "Warning!", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) return;
            var uid = new List<UniqueId>
            {
                new UniqueId(_email.ID)
            };
            _mvm.CurrentUser.CurrentMailbox.Control.DeleteEmailsAsync(uid);
            DialogResult = false;
        }

        private void ReplyBtn_Click(object sender, RoutedEventArgs e)
        {
            var toAddress = _email.SenderAddress;
            var dlg = new ComposeDialog(_mvm, toAddress, Subject);
            DialogResult = false;
            dlg.Show();
        }
    }
}
