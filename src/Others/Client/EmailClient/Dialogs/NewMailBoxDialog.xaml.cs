using System.Windows;
using EmailClient.Core.Model;

using EmailClient.ViewModels;

namespace EmailClient.Dialogs
{
    /// <summary>
    /// Interaction logic for NewMailBoxDialog.xaml
    /// </summary>
    public partial class NewMailBoxDialog : Window
    {
        private readonly MainViewModel _mvm;

        /// <summary>
        /// Gets, sets a MailBoxProperties struct 
        /// </summary>
        public MailBoxProperties Properties { get; private set; }

        /// <summary>
        /// Initializes a new dialog
        /// </summary>
        /// <param name="mvm">The refference of the MainViewModel object</param>
        public NewMailBoxDialog(MainViewModel mvm)
        {
            _mvm = mvm;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// Clicking on the ok button, we confirm the data of the new Mailbox
        /// From the data it creates a new MailBoxProperties struct for initializing the new Mailbox object
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Properties = new MailBoxProperties(
                _mvm.CurrentUser.Name,
                nameField.Text,
                imapField.Text,
                int.Parse(imapPortField.Text),
                smtpField.Text,
                int.Parse(smtpPortField.Text),
                accountField.Text,
                passwordField.Text,
                int.Parse(intervallField.Text)
            );
            var newMailbox = _mvm.CurrentUser.AddMailbox(Properties);
            DialogResult = true;
        }

        /// <summary>
        /// Clicking on the cancel button, we cancel the creational process of a new Mailbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
